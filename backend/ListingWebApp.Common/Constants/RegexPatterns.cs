using System.Text.RegularExpressions;

namespace ListingWebApp.Common.Constants;

public static partial class RegexPatterns
{
    public const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    [GeneratedRegex(EmailPattern)] public static partial Regex EmailRegex();

    public const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9])\S{8,}$";
    [GeneratedRegex(PasswordPattern)] public static partial Regex PasswordRegex();
}
