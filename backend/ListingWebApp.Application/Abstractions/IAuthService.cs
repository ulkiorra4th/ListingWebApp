namespace ListingWebApp.Application.Abstractions;

public interface IAuthService
{
    Task<string> Login(string email, string password);
    Task<string> Register(string email, string password);
    Task Logout(Guid userId);
    Task Refresh(Guid userId);
}