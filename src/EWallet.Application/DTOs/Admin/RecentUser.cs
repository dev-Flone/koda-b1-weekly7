using System.Globalization;

public record class RecentUser(
    long Id,
    string UserName,
    string FullName,
    string Email,
    string Status,
    DateTime CreatedAtUtc
);
