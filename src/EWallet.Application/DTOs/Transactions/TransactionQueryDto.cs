using System.ComponentModel.DataAnnotations;
using EWallet.Domain.Enums;

namespace EWallet.Application.Dtos.Transactions;

public sealed record TransactionQueryDto
{
    [Range(1, int.MaxValue)]
    public int Page { get; init; } = 1;

    [Range(1, 100)]
    public int PageSize { get; init; } = 10;

    public string? Search { get; init; }

    public TransactionType? Type { get; init; }

    public TransactionStatus? Status { get; init; }

    public DateOnly? StartDate { get; init; }

    public DateOnly? EndDate { get; init; }

    public string SortBy { get; init; } = "createdAtUtc";

    public string SortOrder { get; init; } = "desc";
}


