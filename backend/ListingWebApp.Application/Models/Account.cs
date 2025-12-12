using FluentResults;
using ListingWebApp.Common.Constants;
using ListingWebApp.Common.Enums;
using ListingWebApp.Common.Errors;

namespace ListingWebApp.Application.Models;

public sealed class Account
{
    public Guid Id { get; }

    public string Email { get; }
    public string PasswordHash { get; }
    public string Salt { get; }
    
    public AccountStatus Status { get; }
    public AccountRole Role { get; }
    
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }

    private Account(
        Guid id, 
        string email, 
        string passwordHash, 
        string salt, 
        AccountStatus status, 
        AccountRole role,
        DateTime createdAt,
        DateTime updatedAt)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        Salt = salt;
        Status = status;
        Role = role;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static Result<Account> Create(
        string email, 
        string passwordHash, 
        string salt)
    {
        if (!RegexPatterns.EmailRegex().IsMatch(email)) 
            return Result.Fail<Account>(new ValidationError(nameof(Account), "Invalid email."));
        
        if (string.IsNullOrWhiteSpace(passwordHash)) 
            return Result.Fail<Account>( new ValidationError(nameof(Account), "Password is required."));
       
        if (string.IsNullOrWhiteSpace(salt)) 
            return Result.Fail<Account>(new ValidationError(nameof(Account), "Salt is required."));
        
        var now = DateTime.UtcNow;
        return Result.Ok(new Account(
            id:  Guid.NewGuid(), 
            email: email,
            passwordHash: passwordHash, 
            salt: salt,
            role: AccountRole.User,
            status: AccountStatus.Unverified,
            createdAt: now,
            updatedAt: now)
        );
    }

    public static Result<Account> Create(
        Guid id,
        string email,
        string passwordHash,
        string salt,
        AccountRole role,
        AccountStatus status,
        DateTime createdAt,
        DateTime updatedAt)
    {
        return Result.Ok(new Account(
            id: id, 
            email: email, 
            passwordHash: passwordHash, 
            salt: salt, 
            role: role,
            status: status, 
            createdAt: createdAt,
            updatedAt: updatedAt));
    }
}