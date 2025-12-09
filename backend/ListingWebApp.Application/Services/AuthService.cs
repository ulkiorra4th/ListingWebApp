using ListingWebApp.Application.Abstractions;

namespace ListingWebApp.Application.Services;

internal sealed class AuthService : IAuthService
{
    public async Task<string> Login(string email, string password)
    {
        throw new NotImplementedException();
    }

    public async Task<string> Register(string email, string password)
    {
        throw new NotImplementedException();
    }

    public async Task Logout(Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task Refresh(Guid userId)
    {
        throw new NotImplementedException();
    }
}