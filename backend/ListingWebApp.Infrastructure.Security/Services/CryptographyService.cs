using System.Security.Cryptography;
using System.Text;
using ListingWebApp.Application.Contracts.Infrastructure;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ListingWebApp.Infrastructure.Security.Services;

internal sealed class CryptographyService : ICryptographyService
{
    public (string Hash, string Salt) HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password)) return (string.Empty, string.Empty);
        
        var salt = new byte[32];
        RandomNumberGenerator.Fill(salt);

        var hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 100_000,
            numBytesRequested: 64);
        
        return (Convert.ToBase64String(hash),  Convert.ToBase64String(salt));
    }

    public bool VerifyPassword(string hash, string salt, string password)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var expectedHash = Convert.FromBase64String(hash);
        
        var actualHash = KeyDerivation.Pbkdf2(
            password: password,
            salt: saltBytes,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 100_000,
            numBytesRequested: expectedHash.Length);
        
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
    
    public string ComputeSha256(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
    
    public string GenerateToken(int tokenLength = 64)
    {
        Span<byte> buffer = stackalloc byte[tokenLength];
        RandomNumberGenerator.Fill(buffer);
        return Convert.ToBase64String(buffer);
    }
}