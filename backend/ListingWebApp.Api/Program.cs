using ListingWebApp.Application.Extensions;
using ListingWebApp.Infrastructure.Caching.Extensions;
using ListingWebApp.Infrastructure.Email.Extensions;
using ListingWebApp.Infrastructure.Security.Extensions;
using ListingWebApp.Persistence.Postgres.Extensions;
using Microsoft.AspNetCore.CookiePolicy;
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
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
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
    .AddEmailService(builder.Configuration)
    .AddCachingService(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
});

app.UseAuthorization();

app.MapControllers();

app.Run();
