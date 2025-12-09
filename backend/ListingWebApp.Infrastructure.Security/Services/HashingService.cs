using System.Security.Cryptography;
using ListingWebApp.Application.Contracts;
using ListingWebApp.Application.Contracts.Infrastructure;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ListingWebApp.Infrastructure.Security.Services;

internal sealed class HashingService : IHashingService
{
    public (string hash, string salt) Hash(string password)
    {
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

    public bool Verify(string hash, string salt, string password)
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
}