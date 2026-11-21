using EWallet.Infrastructure.Persistence;
using EWallet.Application.Dtos.Common;
using EWallet.Application.Dtos.Transactions;
using EWallet.Application.Dtos.Transfer;
using EWallet.Domain.Enums;
using EWallet.Domain.Entities;
using EWallet.Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EWallet.Infrastructure.Repositories;

public class TransactionRepository(AppDbContext context) : ITransactionRepository
{
    public async Task<List<Transactions>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Transactions
                .AsNoTracking()
                .OrderByDescending(t => t.CreatedAtUtc)
                .ToListAsync(cancellationToken);
    }

    public async Task<Transactions?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        return await context.Transactions.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Transactions?> GetByReferenceNoAsync(string referenceNo, CancellationToken cancellationToken)
    {
        return await context.Transactions.FirstOrDefaultAsync(t => t.ReferenceNo == referenceNo, cancellationToken);
    }

    public async Task<List<Transactions>> GetByWalletId(long walletId, CancellationToken cancellationToken)
    {
        return await context.Transactions
                .AsNoTracking()
                .Where(t => t.SenderWalletId == walletId || t.ReceiverWalletId == walletId)
                .OrderByDescending(t => t.CreatedAtUtc)
                .ToListAsync(cancellationToken);
    }

    public async Task<List<Transactions>> GetByStatusAsync(TransactionStatus status, CancellationToken cancellationToken)
    {
        return await context.Transactions
                .AsNoTracking()
                .Where(t => t.Status == status)
                .OrderByDescending(t => t.CreatedAtUtc)
                .ToListAsync(cancellationToken);
    }

    public async Task<List<Transactions>> GetByTypeAsync(TransactionType type, CancellationToken cancellationToken)
    {
        return await context.Transactions.AsNoTracking().Where(t => t.Type == type).OrderByDescending(t => t.CreatedAtUtc).ToListAsync(cancellationToken);
    }


    // EXISTS
    public async Task<bool> ExistsByReferenceNoAsync(string referenceNo, CancellationToken cancellationToken)
    {
        return await context.Transactions.AnyAsync(t => t.ReferenceNo == referenceNo, cancellationToken);
    }


    // METHODS
    public async Task AddAsync(Transactions transaction, CancellationToken cancellationToken)
    {
        await context.Transactions.AddAsync(transaction, cancellationToken);
    }

    public void Update(Transactions transaction)
    {
        context.Transactions.Update(transaction);
    }


    // Get History
    public async Task<PaginationResponse<TransactionResponseDto>> GetHistoryAsync(TransactionQueryDto query, CancellationToken cancellationToken)
    {
        var transactions = context.Transactions
            .AsNoTracking()
            .AsQueryable();

        // Search by ReferenceNo
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();

            transactions = transactions.Where(t =>
                t.ReferenceNo.Contains(search));
        }

        // Filter by Type
        if (query.Type.HasValue)
        {
            transactions = transactions.Where(t =>
                t.Type == query.Type.Value);
        }

        // Filter by Status
        if (query.Status.HasValue)
        {
            transactions = transactions.Where(t =>
                t.Status == query.Status.Value);
        }

        // Filter by date
        if (query.StartDate.HasValue)
        {
            var startDate = query.StartDate.Value
                .ToDateTime(TimeOnly.MinValue);

            transactions = transactions.Where(t =>
                t.CreatedAtUtc >= startDate);
        }

        if (query.EndDate.HasValue)
        {
            var endDateExclusive = query.EndDate.Value
                .AddDays(1)
                .ToDateTime(TimeOnly.MinValue);

            transactions = transactions.Where(t =>
                t.CreatedAtUtc < endDateExclusive);
        }

        // Count before pagination
        var totalItems = await transactions.CountAsync(cancellationToken);

        // Pagination
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var skip = (int)Math.Min((long)(page - 1) * pageSize, int.MaxValue);

        // Sorting
        var sortBy = (query.SortBy ?? "createdAtUtc")
            .Trim()
            .ToLowerInvariant();

        var ascending = string.Equals(
            query.SortOrder,
            "asc",
            StringComparison.OrdinalIgnoreCase);

        var sortedTransactions = sortBy switch
        {
            "referenceno" => ascending
                ? transactions.OrderBy(t => t.ReferenceNo)
                : transactions.OrderByDescending(t => t.ReferenceNo),

            "amount" => ascending
                ? transactions.OrderBy(t => t.Amount)
                : transactions.OrderByDescending(t => t.Amount),

            "type" => ascending
                ? transactions.OrderBy(t => t.Type)
                : transactions.OrderByDescending(t => t.Type),

            "status" => ascending
                ? transactions.OrderBy(t => t.Status)
                : transactions.OrderByDescending(t => t.Status),

            "completedatutc" => ascending
                ? transactions.OrderBy(t => t.CompletedAtUtc)
                : transactions.OrderByDescending(t => t.CompletedAtUtc),

            _ => ascending
                ? transactions.OrderBy(t => t.CreatedAtUtc)
                : transactions.OrderByDescending(t => t.CreatedAtUtc)
        };

        // Projection + Pagination
        var items = await sortedTransactions
            .ThenBy(t => t.Id)
            .Skip(skip)
            .Take(pageSize)
            .Select(t => new TransactionResponseDto
            {
                Id = t.Id,
                ReferenceNo = t.ReferenceNo,
                SenderWalletId = t.SenderWalletId,
                ReceiverWalletId = t.ReceiverWalletId,
                Type = t.Type.ToString(),
                Amount = t.Amount,
                Status = t.Status.ToString(),
                Description = t.Description,
                CreatedAtUtc = t.CreatedAtUtc,
                CompletedAtUtc = t.CompletedAtUtc
            })
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(
            (double)totalItems / pageSize);

        return new PaginationResponse<TransactionResponseDto>(
            items,
            page,
            pageSize,
            totalItems,
            totalPages);
    }
}


