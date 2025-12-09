namespace ListingWebApp.Application.Contracts.Infrastructure;

public interface ICryptographyService
{
    (string Hash, string Salt) HashPassword(string password);
    bool VerifyPassword(string hash, string salt, string password);
    string ComputeSha256(string token);
    string GenerateToken(int tokenLength = 64);
}