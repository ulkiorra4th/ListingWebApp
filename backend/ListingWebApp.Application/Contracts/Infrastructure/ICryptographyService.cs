namespace ListingWebApp.Application.Contracts.Infrastructure;

public interface ICryptographyService
{
    (string Hash, string Salt) HashSecret(string password);
    bool VerifySecret(string hash, string salt, string password);
    string ComputeSha256(string token);
    string GenerateCode(int codeLength = 6);
    string GenerateToken(int tokenLength = 64);
}