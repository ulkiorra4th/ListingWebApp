using System.Text;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Dto.Request;
using Microsoft.Extensions.DependencyInjection;

namespace ListingWebApp.Application.Tests;

public sealed class AuthTests
{
    private const string Chars = "abcdefghijklmnopqrstuvwxyz";
    private const string InvalidChars = @"!@#$%^&*()+=<>?/\|!@#$%'`~ ";
    private const string Numbers = "0123456789";
    private const int CountOfGeneratedTestData = 10;
    private const int EmailLength = 12;
    private const int PasswordLength = 16;
    
    private readonly IAuthService _authService;

    public AuthTests(IAuthService authService)
    {
        _authService = Services.Provider.GetRequiredService<IAuthService>();
    }
    
    [Theory]
    [MemberData(nameof(AccountRegisterData))]
    public async Task RegisterSuccessTest(string email, string password)
    {
        var dto = new LoginRequestDto(email, password);
        var tokenResult = await _authService.RegisterAsync(dto);
        Assert.True(tokenResult.IsSuccess);
    }
    
    public static IEnumerable<object[]> AccountRegisterData()
    {
        for (int i = 0; i < CountOfGeneratedTestData; i++)
        {
            var email = GenerateEmail();
            var password = GeneratePassword();
            
            yield return [email, password];
        }
    }

    private static string GenerateEmail(bool isInvalid = false)
    {
        var randomString = new StringBuilder(EmailLength);
        for (int j = 0; j < EmailLength; ++j)
        {
            randomString.Append(Chars[Random.Shared.Next(Chars.Length)]);
        }

        if (isInvalid)
        {
            randomString.Append(InvalidChars[Random.Shared.Next(InvalidChars.Length)]);
        }

        return randomString + "@gmail.com";
    }

    private static string GeneratePassword(bool isInvalid = false)
    {
        var randomString = new StringBuilder(PasswordLength);
        for (int j = 0; j < PasswordLength / 2 - 1; ++j)
        {
            randomString.Append(Chars[Random.Shared.Next(Chars.Length)]);
        }

        randomString.Append(Numbers[Random.Shared.Next(Numbers.Length)]);
        
        var password = randomString.ToString() + randomString.ToString().ToUpper();
        return isInvalid ? password.ToLower() : password;
    }
}