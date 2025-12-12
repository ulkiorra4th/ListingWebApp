using System.Security.Claims;
using System.Text;
using ListingWebApp.Api.BackgroundServices;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Extensions;
using ListingWebApp.Infrastructure.Caching.Extensions;
using ListingWebApp.Infrastructure.Email.Extensions;
using ListingWebApp.Infrastructure.Security.Extensions;
using ListingWebApp.Infrastructure.Security.Options;
using ListingWebApp.Infrastructure.Storage.Extensions;
using ListingWebApp.Persistence.Postgres.Connection;
using ListingWebApp.Persistence.Postgres.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddMvc();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddApplication()
    .AddRepositories(builder.Configuration.GetConnectionString("Postgres")!)
    .AddSecurityServices(builder.Configuration)
    .AddEmailService(builder.Configuration, builder.Environment.ContentRootPath)
    .AddCachingService(builder.Configuration)
    .AddObjectStorage(builder.Configuration);

builder.Services.AddHostedService<VerificationMessageSenderBackgroundService>();

var jwtOptions = builder.Configuration
    .GetSection("JwtOptions")
    .Get<JwtOptions>()!;

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            
            ValidateIssuer = false,
            ValidateAudience = false,

            ClockSkew = TimeSpan.Zero,
            
            RoleClaimType = ClaimTypes.Role
        };
        
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var user = context.Principal;
                
                var sessionIdStr = user?.FindFirstValue("sessionId");
                if (!Guid.TryParse(sessionIdStr, out var sessionId))
                {
                    context.Fail("Invalid session id");
                    return;
                }
                
                var sessionsRepository = context.HttpContext.RequestServices
                    .GetRequiredService<ISessionsRepository>();

                var sessionResult = await sessionsRepository.GetSessionByIdAsync(sessionId);
                if (sessionResult.IsFailed)
                {
                    context.Fail("Session not found");
                    return;
                }

                var session = sessionResult.Value;

                if (!session.IsActive || session.ExpiresAt <= DateTime.UtcNow)
                {
                    context.Fail("Session revoked or expired");
                }
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();
        await db.Database.MigrateAsync();
    }
}

app.UseCors("Frontend");

app.UseCookiePolicy(new CookiePolicyOptions
{
    HttpOnly = HttpOnlyPolicy.Always
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
