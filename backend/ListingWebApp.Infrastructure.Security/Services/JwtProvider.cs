using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ListingWebApp.Application.Contracts;
using ListingWebApp.Application.Contracts.Infrastructure;
using ListingWebApp.Application.Models;
using ListingWebApp.Infrastructure.Security.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ListingWebApp.Infrastructure.Security.Services;

internal sealed class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _options;
    
    public JwtProvider(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }
    
    public string GenerateToken(Account account)
    {
        Claim[] claims = 
        {
            new("accountId", account.Id.ToString()), 
            new("accountEmail", account.Email),
            new("accountStatus", account.Status.ToString())
        };
        
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpiresMinutes));

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenValue;
    }
}