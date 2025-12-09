using FluentResults;
using ListingWebApp.Common.Constants;
using ListingWebApp.Common.Enums;
using ListingWebApp.Common.Errors;

namespace ListingWebApp.Application.Models;

public sealed class Account
{
    public Guid Id { get; private set; }

    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Salt { get; private set; }
    
    public AccountStatus Status { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Account(
        Guid id, 
        string email, 
        string passwordHash, 
        string salt, 
        AccountStatus status, 
        DateTime createdAt,
        DateTime updatedAt)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        Salt = salt;
        Status = status;
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
        AccountStatus status,
        DateTime createdAt,
        DateTime updatedAt)
    {
        return Result.Ok(new Account(
            id: id, 
            email: email, 
            passwordHash: passwordHash, 
            salt: salt, 
            status: status, 
            createdAt: createdAt,
            updatedAt: updatedAt));
    }
}