using System.Text.RegularExpressions;

namespace ListingWebApp.Common.Constants;

public static partial class RegexPatterns
{
    public const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    [GeneratedRegex(EmailPattern)] public static partial Regex EmailRegex();
}
