using FluentResults;
using ListingWebApp.Application.Abstractions;
using ListingWebApp.Application.Contracts.Infrastructure;
using ListingWebApp.Application.Contracts.Persistence;
using ListingWebApp.Application.Dto.Messages;
using ListingWebApp.Application.Dto.Request;
using ListingWebApp.Application.Dto.Response;
using ListingWebApp.Application.Models;
using ListingWebApp.Common.Constants;
using ListingWebApp.Common.Enums;
using ListingWebApp.Common.Errors;
using Microsoft.Extensions.Logging;

namespace ListingWebApp.Application.Services;

internal sealed class AuthService : IAuthService
{
    private const int RefreshTokenExpiresDays = 30;
    private const int VerificationCodeLength = 6;

    private readonly IJwtProvider _jwtProvider;
    private readonly IAccountsRepository _accountsRepository;
    private readonly ISessionsRepository _sessionsRepository;
    private readonly ICryptographyService _cryptographyService;
    private readonly ICacheService _cacheService;
    private readonly IAccountVerificationQueue _accountVerificationQueue;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IJwtProvider jwtProvider,
        IAccountsRepository accountsRepository,
        ICryptographyService cryptographyService,
        ISessionsRepository sessionsRepository,
        ICacheService cacheService,
        IAccountVerificationQueue accountVerificationQueue, 
        ILogger<AuthService> logger)
    {
        _jwtProvider = jwtProvider;
        _accountsRepository = accountsRepository;
        _cryptographyService = cryptographyService;
        _sessionsRepository = sessionsRepository;
        _cacheService = cacheService;
        _accountVerificationQueue = accountVerificationQueue;
        _logger = logger;
    }

    public async Task<Result<LoginResponseDto>> LoginAsync(string email, string password)
    {
        var accountResult = await _accountsRepository.GetAccountByEmailAsync(email);
        if (accountResult.IsFailed)
        {
            _logger.LogError("Login failed for user with email {email}. Errors: {errors}", 
                email, string.Join(';', accountResult.Errors));
            return Result.Fail<LoginResponseDto>(accountResult.Errors);
        }

        var account = accountResult.Value;

        var isPasswordValid = _cryptographyService.VerifySecret(account.PasswordHash, account.Salt, password);
        if (!isPasswordValid)
        {
            _logger.LogError("Login failed for user with email {email}. Errors: {errors}", 
                email, "password is incorrect.");
            return Result.Fail<LoginResponseDto>(new ValidationError(nameof(Account), "Invalid credentials"));
        }

        var refreshToken = _cryptographyService.GenerateToken();
        var refreshHash = _cryptographyService.ComputeSha256(refreshToken);
        var refreshExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpiresDays);

        var sessionResult = await _sessionsRepository.CreateSessionAsync(
            account,
            refreshHash,
            refreshExpiresAt);

        if (sessionResult.IsFailed)
        {
            _logger.LogError("Login failed for user with email {email}. Errors: {errors}", 
                email, string.Join(';', sessionResult.Errors));
            return Result.Fail<LoginResponseDto>(sessionResult.Errors);
        }

        _logger.LogInformation("User with email {email} logged in.", email);
        
        var accessToken = _jwtProvider.GenerateToken(account, sessionResult.Value);
        return Result.Ok(new LoginResponseDto(accessToken, refreshToken, accountResult.Value.Id.ToString()));
    }

    public async Task<Result<LoginResponseDto>> RegisterAsync(LoginRequestDto dto)
    {
        if (!RegexPatterns.PasswordRegex().IsMatch(dto.Password))
        {
            _logger.LogError("Registration failed for user with email {email}. Errors: {errors}", 
                dto.Email, "invalid password.");
            return Result.Fail<LoginResponseDto>(new ValidationError(nameof(Account), "Bad password"));
        }
        
        var hashResult = _cryptographyService.HashSecret(dto.Password);

        var accountResult = Account.Create(dto.Email, hashResult.Hash, hashResult.Salt);
        if (accountResult.IsFailed)
        {
            _logger.LogError("Registration failed for user with email {email}. Errors: {errors}", 
                dto.Email, string.Join(';', accountResult.Errors));
            return Result.Fail<LoginResponseDto>(accountResult.Errors);
        }

        var createResult = await _accountsRepository.CreateAccountAsync(accountResult.Value);
        if (createResult.IsFailed)
        {
            _logger.LogError("Registration failed for user with email {email}. Errors: {errors}", 
                dto.Email, string.Join(';', createResult.Errors));
            return Result.Fail<LoginResponseDto>(createResult.Errors);
        }

        var refreshToken = _cryptographyService.GenerateToken();
        var refreshHash = _cryptographyService.ComputeSha256(refreshToken);
        var refreshExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpiresDays);

        var sessionResult = await _sessionsRepository.CreateSessionAsync(
            accountResult.Value,
            refreshHash,
            refreshExpiresAt
        );

        if (sessionResult.IsFailed)
        {
            _logger.LogError("Registration failed for user with email {email}. Errors: {errors}", 
                dto.Email, string.Join(';', sessionResult.Errors));
            return Result.Fail<LoginResponseDto>(sessionResult.Errors);
        }

        var verificationCode = _cryptographyService.GenerateCode(codeLength: VerificationCodeLength);
        var codeHashResult = _cryptographyService.HashSecret(verificationCode);

        var verificationSecret = new VerificationSecretData(codeHashResult.Hash, codeHashResult.Salt);
        var cacheCodeResult = await _cacheService.ReplaceAsync(accountResult.Value.Id.ToString(), verificationSecret, 10);
        if (cacheCodeResult.IsFailed)
        {
            _logger.LogError("Registration failed for user with email {email}. Errors: {errors}", 
                dto.Email, string.Join(';', cacheCodeResult.Errors));
            return Result.Fail<LoginResponseDto>(cacheCodeResult.Errors);
        }
        
        await _accountVerificationQueue.QueueAsync(new AccountVerificationMessageDto(
            dto.Email,
            verificationCode)
        );

        _logger.LogInformation("User with email {email} successfully registered.", dto.Email);
        
        var accessToken = _jwtProvider.GenerateToken(accountResult.Value, sessionResult.Value);
        return Result.Ok(new LoginResponseDto(accessToken, refreshToken, accountResult.Value.Id.ToString()));
    }

    public async Task<Result> VerifyAccountAsync(Guid accountId, string code)
    {
        var secretResult = await _cacheService.GetAsync<VerificationSecretData>(accountId.ToString());
        if (secretResult.IsFailed)
        {
            return Result.Fail(secretResult.Errors);
        }
        
        var verifyResult = _cryptographyService.VerifySecret(secretResult.Value.CodeHash, secretResult.Value.Salt, code);
        if (!verifyResult)
        {
            return Result.Fail(new ValidationError("Invalid code"));
        }

        await _cacheService.RemoveAsync(accountId.ToString());
        
        var updateResult = await _accountsRepository.UpdateStatusAsync(accountId, AccountStatus.Verified);
        return updateResult.IsFailed 
            ? Result.Fail(updateResult.Errors) 
            : Result.Ok();
    }
    
    public async Task<Result> LogoutAsync(Guid userId)
    {
        return await _sessionsRepository.DeleteSessionByAccountIdAsync(userId);
    }

    public async Task<Result<LoginResponseDto>> RefreshAsync(string refreshToken)
    {
        var refreshHash = _cryptographyService.ComputeSha256(refreshToken);
        var sessionResult = await _sessionsRepository.GetSessionByRefreshTokenHashAsync(refreshHash);
        if (sessionResult.IsFailed)
            return Result.Fail<LoginResponseDto>(sessionResult.Errors);

        var session = sessionResult.Value;
        if (!session.IsActive || session.ExpiresAt <= DateTime.UtcNow)
        {
            return Result.Fail<LoginResponseDto>(new ValidationError(nameof(Session),
                "Session is inactive or expired"));
        }

        var accountResult = await _accountsRepository.GetAccountByIdAsync(session.AccountId);
        if (accountResult.IsFailed)
        {
            return Result.Fail<LoginResponseDto>(accountResult.Errors);
        }

        var newRefreshToken = _cryptographyService.GenerateToken();
        var newRefreshHash = _cryptographyService.ComputeSha256(newRefreshToken);
        var refreshExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpiresDays);

        var updatedSessionResult = Session.Create(
            id: session.Id,
            accountId: session.AccountId,
            refreshTokenHash: newRefreshHash,
            isActive: true,
            createdAt: session.CreatedAt,
            updatedAt: DateTime.UtcNow,
            expiresAt: refreshExpiresAt,
            revokedAt: null);

        if (updatedSessionResult.IsFailed)
        {
            return Result.Fail<LoginResponseDto>(updatedSessionResult.Errors);
        }

        var updateResult = await _sessionsRepository.UpdateSessionAsync(updatedSessionResult.Value);
        if (updateResult.IsFailed)
        {
            return Result.Fail<LoginResponseDto>(updateResult.Errors);
        }

        var accessToken = _jwtProvider.GenerateToken(accountResult.Value, sessionResult.Value.Id);
        return Result.Ok(new LoginResponseDto(accessToken, newRefreshToken, accountResult.Value.Id.ToString()));
    }
}