
using EWallet.Domain.Entities;
using EWallet.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EWallet.Infrastructure.Seeding;

public static class TransactionSeedData
{
    private const string SeedPrefix = "SEED-";

    private const string TopUpSuccess = "SEED-TOPUP-001";
    private const string TopUpPending = "SEED-TOPUP-002";
    private const string TopUpFailed = "SEED-TOPUP-003";
    private const string TransferSuccess = "SEED-TRANSFER-001";
    private const string TransferFailed = "SEED-TRANSFER-002";

    public static void Seed(DbContext context)
    {
        var (senderWallet, receiverWallet) = ResolveSeedWallets(context);

        var (bankTransfer, qris) = GetPaymentMethods(context);

        var now = DateTime.UtcNow;

        var transactions = GetOrCreateTransactions(
            context,
            senderWallet,
            receiverWallet,
            now);

        EnsureWalletBalances(context, senderWallet, receiverWallet);

        CreateTopUpRequests(
            context,
            transactions,
            senderWallet,
            bankTransfer,
            qris,
            now);

        CreateMutations(
            context,
            transactions,
            senderWallet,
            receiverWallet);

        CreateAuditLogs(
            context,
            transactions,
            senderWallet);

        context.SaveChanges();
    }

    public static async Task SeedAsync(
        DbContext context,
        CancellationToken cancellationToken)
    {
        var (senderWallet, receiverWallet) =
            await ResolveSeedWalletsAsync(context, cancellationToken);

        var (bankTransfer, qris) =
            await GetPaymentMethodsAsync(context, cancellationToken);

        var now = DateTime.UtcNow;

        var transactions = await GetOrCreateTransactionsAsync(
            context,
            senderWallet,
            receiverWallet,
            now,
            cancellationToken);

        await EnsureWalletBalancesAsync(
            context,
            senderWallet,
            receiverWallet,
            cancellationToken);

        await CreateTopUpRequestsAsync(
            context,
            transactions,
            senderWallet,
            bankTransfer,
            qris,
            now,
            cancellationToken);

        await CreateMutationsAsync(
            context,
            transactions,
            senderWallet,
            receiverWallet,
            cancellationToken);

        await CreateAuditLogsAsync(
            context,
            transactions,
            senderWallet,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    // WALLET RESOLUTION

    private static (
        Wallets Sender,
        Wallets Receiver
    ) ResolveSeedWallets(DbContext context)
    {
        var seedTransactions = context.Set<Transactions>()
            .Where(t => t.ReferenceNo.StartsWith(SeedPrefix))
            .ToList();

        var wallets = context.Set<Wallets>().ToList();

        return ResolveWalletPair(
            context,
            seedTransactions,
            wallets);
    }

    private static async Task<(
        Wallets Sender,
        Wallets Receiver
    )> ResolveSeedWalletsAsync(
        DbContext context,
        CancellationToken cancellationToken)
    {
        var seedTransactions = await context.Set<Transactions>()
            .Where(t => t.ReferenceNo.StartsWith(SeedPrefix))
            .ToListAsync(cancellationToken);

        var wallets = await context.Set<Wallets>()
            .ToListAsync(cancellationToken);

        return ResolveWalletPair(
            context,
            seedTransactions,
            wallets);
    }

    private static (
        Wallets Sender,
        Wallets Receiver
    ) ResolveWalletPair(
        DbContext context,
        List<Transactions> transactions,
        List<Wallets> wallets)
    {
        Wallets? sender = null;
        Wallets? receiver = null;

        var transfer = transactions.FirstOrDefault(
            t => t.Type == TransactionType.TRANSFER);

        if (transfer is not null)
        {
            if (transfer.SenderWalletId is null)
                throw new InvalidOperationException(
                    "Seed transfer is missing SenderWalletId.");

            sender = wallets.FirstOrDefault(
                w => w.Id == transfer.SenderWalletId.Value);

            receiver = wallets.FirstOrDefault(
                w => w.Id == transfer.ReceiverWalletId);
        }

        if (sender is null)
        {
            var topUp = transactions.FirstOrDefault(
                t => t.ReferenceNo == TopUpSuccess ||
                     t.ReferenceNo == TopUpPending ||
                     t.ReferenceNo == TopUpFailed);

            if (topUp is not null)
            {
                sender = wallets.FirstOrDefault(
                    w => w.Id == topUp.ReceiverWalletId);
            }
        }

        if (sender is null)
        {
            var seedUserIds = context.Set<Users>()
                .Where(u => u.UserName.StartsWith("seed."))
                .Select(u => u.Id)
                .ToHashSet();

            var seedWallets = wallets
                .Where(w => seedUserIds.Contains(w.UserId))
                .OrderBy(w => w.Id)
                .ToList();

            if (seedWallets.Count < 2)
                throw new InvalidOperationException(
                    "At least two seed users with wallets are required.");

            sender = seedWallets[0];
            receiver = seedWallets[1];
        }

        if (receiver is null)
        {
            var seedUserIds = context.Set<Users>()
                .Where(u => u.UserName.StartsWith("seed."))
                .Select(u => u.Id)
                .ToHashSet();

            receiver = wallets
                .Where(w =>
                    w.Id != sender.Id &&
                    seedUserIds.Contains(w.UserId))
                .OrderBy(w => w.Id)
                .FirstOrDefault();
        }

        if (receiver is null || sender.Id == receiver.Id)
            throw new InvalidOperationException(
                "Unable to resolve two different seed wallets.");

        if (sender.Status != WalletStatus.ACTIVE ||
            receiver.Status != WalletStatus.ACTIVE)
            throw new InvalidOperationException(
                "Seed wallets must have ACTIVE status.");

        return (sender, receiver);
    }

    // PAYMENT METHODS

    private static (
        PaymentMethods BankTransfer,
        PaymentMethods Qris
    ) GetPaymentMethods(DbContext context)
    {
        var bankTransfer = context.Set<PaymentMethods>()
            .FirstOrDefault(pm =>
                pm.Code == PaymentMethodCode.BANK_TRANSFER &&
                pm.IsActive);

        var qris = context.Set<PaymentMethods>()
            .FirstOrDefault(pm =>
                pm.Code == PaymentMethodCode.QRIS &&
                pm.IsActive);

        if (bankTransfer is null || qris is null)
            throw new InvalidOperationException(
                "Required payment methods were not found.");

        return (bankTransfer, qris);
    }

    private static async Task<(
        PaymentMethods BankTransfer,
        PaymentMethods Qris
    )> GetPaymentMethodsAsync(
        DbContext context,
        CancellationToken cancellationToken)
    {
        var bankTransfer = await context.Set<PaymentMethods>()
            .FirstOrDefaultAsync(
                pm => pm.Code == PaymentMethodCode.BANK_TRANSFER &&
                      pm.IsActive,
                cancellationToken);

        var qris = await context.Set<PaymentMethods>()
            .FirstOrDefaultAsync(
                pm => pm.Code == PaymentMethodCode.QRIS &&
                      pm.IsActive,
                cancellationToken);

        if (bankTransfer is null || qris is null)
            throw new InvalidOperationException(
                "Required payment methods were not found.");

        return (bankTransfer, qris);
    }

    // TRANSACTIONS

    private static Transactions[] CreateTransactions(
        Wallets sender,
        Wallets receiver,
        DateTime now)
    {
        return
        [
            new Transactions(
                TopUpSuccess,
                sender.Id,
                500000m,
                TransactionType.TOPUP,
                status: TransactionStatus.SUCCESS,
                description: "Dummy successful top up",
                completedAtUtc: now
            ),

            new Transactions(
                TopUpPending,
                sender.Id,
                100000m,
                TransactionType.TOPUP,
                status: TransactionStatus.PENDING,
                description: "Dummy pending top up"
            ),

            new Transactions(
                TopUpFailed,
                sender.Id,
                50000m,
                TransactionType.TOPUP,
                status: TransactionStatus.FAILED,
                description: "Dummy failed top up",
                completedAtUtc: now
            ),

            new Transactions(
                TransferSuccess,
                receiver.Id,
                75000m,
                TransactionType.TRANSFER,
                senderWalletId: sender.Id,
                status: TransactionStatus.SUCCESS,
                description: "Dummy successful transfer",
                completedAtUtc: now
            ),

            new Transactions(
                TransferFailed,
                receiver.Id,
                25000m,
                TransactionType.TRANSFER,
                senderWalletId: sender.Id,
                status: TransactionStatus.FAILED,
                description: "Dummy failed transfer",
                completedAtUtc: now
            )
        ];
    }

    private static Transactions[] GetOrCreateTransactions(
        DbContext context,
        Wallets sender,
        Wallets receiver,
        DateTime now)
    {
        var existing = context.Set<Transactions>()
            .Where(t => t.ReferenceNo.StartsWith(SeedPrefix))
            .ToDictionary(t => t.ReferenceNo);

        var expected = CreateTransactions(sender, receiver, now);

        var missing = expected
            .Where(t => !existing.ContainsKey(t.ReferenceNo))
            .ToList();

        context.Set<Transactions>().AddRange(missing);

        context.SaveChanges();

        return context.Set<Transactions>()
            .Where(t => t.ReferenceNo.StartsWith(SeedPrefix))
            .ToArray();
    }

    private static async Task<Transactions[]> GetOrCreateTransactionsAsync(
        DbContext context,
        Wallets sender,
        Wallets receiver,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var existing = await context.Set<Transactions>()
            .Where(t => t.ReferenceNo.StartsWith(SeedPrefix))
            .ToDictionaryAsync(
                t => t.ReferenceNo,
                cancellationToken);

        var expected = CreateTransactions(sender, receiver, now);

        var missing = expected
            .Where(t => !existing.ContainsKey(t.ReferenceNo))
            .ToList();

        await context.Set<Transactions>()
            .AddRangeAsync(missing, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return await context.Set<Transactions>()
            .Where(t => t.ReferenceNo.StartsWith(SeedPrefix))
            .ToArrayAsync(cancellationToken);
    }

    // WALLET BALANCES

    private static void EnsureWalletBalances(
        DbContext context,
        Wallets sender,
        Wallets receiver)
    {
        var expectedReferences = new[]
        {
            TopUpSuccess,
            TransferSuccess
        };

        var transactionIds = context.Set<Transactions>()
            .Where(t => expectedReferences.Contains(t.ReferenceNo))
            .Select(t => t.Id)
            .ToHashSet();

        var mutations = context.Set<WalletMutations>()
            .Where(m => transactionIds.Contains(m.TransactionId))
            .ToList();

        if (mutations.Count > 0)
        {
            if (mutations.Count != 3)
                throw new InvalidOperationException(
                    "Seed wallet mutations are incomplete. " +
                    "Wallet balances were not modified.");

            return;
        }

        var hasOtherMutations = context.Set<WalletMutations>()
            .Any(m =>
                m.WalletId == sender.Id ||
                m.WalletId == receiver.Id);

        var hasOtherTransactions = context.Set<Transactions>()
            .Any(t =>
                !t.ReferenceNo.StartsWith(SeedPrefix) &&
                (t.SenderWalletId == sender.Id ||
                 t.ReceiverWalletId == sender.Id ||
                 t.SenderWalletId == receiver.Id ||
                 t.ReceiverWalletId == receiver.Id));

        if (hasOtherMutations || hasOtherTransactions ||
            sender.Balance != 0m || receiver.Balance != 0m)
        {
            throw new InvalidOperationException(
                "Cannot initialize seed balances safely because " +
                "wallets already contain other activity.");
        }

        sender.Credit(500000m);
        sender.Debit(75000m);
        receiver.Credit(75000m);
    }

    private static async Task EnsureWalletBalancesAsync(
        DbContext context,
        Wallets sender,
        Wallets receiver,
        CancellationToken cancellationToken)
    {
        var expectedReferences = new[]
        {
            TopUpSuccess,
            TransferSuccess
        };

        var transactionIds = await context.Set<Transactions>()
            .Where(t => expectedReferences.Contains(t.ReferenceNo))
            .Select(t => t.Id)
            .ToListAsync(cancellationToken);

        var mutations = await context.Set<WalletMutations>()
            .Where(m => transactionIds.Contains(m.TransactionId))
            .ToListAsync(cancellationToken);

        if (mutations.Count > 0)
        {
            if (mutations.Count != 3)
                throw new InvalidOperationException(
                    "Seed wallet mutations are incomplete. " +
                    "Wallet balances were not modified.");

            return;
        }

        var hasOtherMutations = await context.Set<WalletMutations>()
            .AnyAsync(
                m => m.WalletId == sender.Id ||
                     m.WalletId == receiver.Id,
                cancellationToken);

        var hasOtherTransactions = await context.Set<Transactions>()
            .AnyAsync(
                t => !t.ReferenceNo.StartsWith(SeedPrefix) &&
                     (t.SenderWalletId == sender.Id ||
                      t.ReceiverWalletId == sender.Id ||
                      t.SenderWalletId == receiver.Id ||
                      t.ReceiverWalletId == receiver.Id),
                cancellationToken);

        if (hasOtherMutations || hasOtherTransactions ||
            sender.Balance != 0m || receiver.Balance != 0m)
        {
            throw new InvalidOperationException(
                "Cannot initialize seed balances safely because " +
                "wallets already contain other activity.");
        }

        sender.Credit(500000m);
        sender.Debit(75000m);
        receiver.Credit(75000m);
    }

    // TOP UP REQUESTS

    private static void CreateTopUpRequests(
        DbContext context,
        Transactions[] transactions,
        Wallets sender,
        PaymentMethods bankTransfer,
        PaymentMethods qris,
        DateTime now)
    {
        var byReference = transactions.ToDictionary(
            t => t.ReferenceNo);

        var expected = CreateTopUpRequests(
            byReference,
            sender,
            bankTransfer,
            qris,
            now);

        var existingIds = context.Set<TopUpRequests>()
            .Select(t => t.TransactionId)
            .ToHashSet();

        var missing = expected
            .Where(t => !existingIds.Contains(t.TransactionId))
            .ToList();

        context.Set<TopUpRequests>().AddRange(missing);
    }

    private static TopUpRequests[] CreateTopUpRequests(
        Dictionary<string, Transactions> transactions,
        Wallets sender,
        PaymentMethods bankTransfer,
        PaymentMethods qris,
        DateTime now)
    {
        var success = transactions[TopUpSuccess];
        var pending = transactions[TopUpPending];
        var failed = transactions[TopUpFailed];

        return
        [
            new TopUpRequests(
                success.Id,
                sender.Id,
                bankTransfer.Id,
                500000m,
                TopUpRequestStatus.PAID,
                "SEED-PAY-001",
                null,
                now
            ),

            new TopUpRequests(
                pending.Id,
                sender.Id,
                qris.Id,
                100000m,
                TopUpRequestStatus.PENDING,
                "SEED-PAY-002",
                now.AddHours(24)
            ),

            new TopUpRequests(
                failed.Id,
                sender.Id,
                bankTransfer.Id,
                50000m,
                TopUpRequestStatus.FAILED,
                "SEED-PAY-003"
            )
        ];
    }

    private static async Task CreateTopUpRequestsAsync(
        DbContext context,
        Transactions[] transactions,
        Wallets sender,
        PaymentMethods bankTransfer,
        PaymentMethods qris,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var byReference = transactions.ToDictionary(
            t => t.ReferenceNo);

        var expected = CreateTopUpRequests(
            byReference,
            sender,
            bankTransfer,
            qris,
            now);

        var existingIds = await context.Set<TopUpRequests>()
            .Select(t => t.TransactionId)
            .ToListAsync(cancellationToken);

        var existingSet = existingIds.ToHashSet();

        var missing = expected
            .Where(t => !existingSet.Contains(t.TransactionId))
            .ToList();

        await context.Set<TopUpRequests>()
            .AddRangeAsync(missing, cancellationToken);
    }

    // WALLET MUTATIONS

    private static WalletMutations[] CreateMutations(
        Transactions[] transactions,
        Wallets sender,
        Wallets receiver)
    {
        var byReference = transactions.ToDictionary(
            t => t.ReferenceNo);

        var topUp = byReference[TopUpSuccess];
        var transfer = byReference[TransferSuccess];

        return
        [
            new WalletMutations(
                topUp.Id,
                sender.Id,
                500000m,
                0m,
                500000m,
                WalletMutationDirection.IN
            ),

            new WalletMutations(
                transfer.Id,
                sender.Id,
                75000m,
                500000m,
                425000m,
                WalletMutationDirection.OUT
            ),

            new WalletMutations(
                transfer.Id,
                receiver.Id,
                75000m,
                0m,
                75000m,
                WalletMutationDirection.IN
            )
        ];
    }

    private static void CreateMutations(
        DbContext context,
        Transactions[] transactions,
        Wallets sender,
        Wallets receiver)
    {
        var expected = CreateMutations(
            transactions,
            sender,
            receiver);

        var existing = context.Set<WalletMutations>()
            .ToList();

        var missing = expected
            .Where(m => !existing.Any(e =>
                e.TransactionId == m.TransactionId &&
                e.WalletId == m.WalletId &&
                e.Direction == m.Direction))
            .ToList();

        context.Set<WalletMutations>().AddRange(missing);
    }

    private static async Task CreateMutationsAsync(
        DbContext context,
        Transactions[] transactions,
        Wallets sender,
        Wallets receiver,
        CancellationToken cancellationToken)
    {
        var expected = CreateMutations(
            transactions,
            sender,
            receiver);

        var existing = await context.Set<WalletMutations>()
            .ToListAsync(cancellationToken);

        var missing = expected
            .Where(m => !existing.Any(e =>
                e.TransactionId == m.TransactionId &&
                e.WalletId == m.WalletId &&
                e.Direction == m.Direction))
            .ToList();

        await context.Set<WalletMutations>()
            .AddRangeAsync(missing, cancellationToken);
    }

    // AUDIT LOGS

    private static AuditLogs[] CreateAuditLogs(
        Transactions[] transactions,
        Wallets sender)
    {
        var byReference = transactions.ToDictionary(
            t => t.ReferenceNo);

        return
        [
            new AuditLogs(
                sender.UserId,
                byReference[TopUpSuccess].Id,
                AuditLogAction.TOPUP_PAID,
                AuditLogEntityType.TRANSACTION,
                byReference[TopUpSuccess].Id.ToString(),
                AuditLogOutcome.SUCCESS,
                "Top up completed successfully",
                TopUpSuccess
            ),

            new AuditLogs(
                sender.UserId,
                byReference[TopUpPending].Id,
                AuditLogAction.TOPUP_REQUESTED,
                AuditLogEntityType.TRANSACTION,
                byReference[TopUpPending].Id.ToString(),
                AuditLogOutcome.SUCCESS,
                "Top up request created and awaiting payment",
                TopUpPending
            ),

            new AuditLogs(
                sender.UserId,
                byReference[TopUpFailed].Id,
                AuditLogAction.TOPUP_REQUESTED,
                AuditLogEntityType.TRANSACTION,
                byReference[TopUpFailed].Id.ToString(),
                AuditLogOutcome.FAILED,
                "Top up transaction failed",
                TopUpFailed
            ),

            new AuditLogs(
                sender.UserId,
                byReference[TransferSuccess].Id,
                AuditLogAction.TRANSFER_CREATED,
                AuditLogEntityType.TRANSACTION,
                byReference[TransferSuccess].Id.ToString(),
                AuditLogOutcome.SUCCESS,
                "Transfer completed successfully",
                TransferSuccess
            ),

            new AuditLogs(
                sender.UserId,
                byReference[TransferFailed].Id,
                AuditLogAction.TRANSFER_REJECTED,
                AuditLogEntityType.TRANSACTION,
                byReference[TransferFailed].Id.ToString(),
                AuditLogOutcome.FAILED,
                "Transfer transaction failed",
                TransferFailed
            )
        ];
    }

    private static void CreateAuditLogs(
        DbContext context,
        Transactions[] transactions,
        Wallets sender)
    {
        var expected = CreateAuditLogs(transactions, sender);

        var existingIds = context.Set<AuditLogs>()
            .Where(a => a.CorrelationId != null)
            .Select(a => a.CorrelationId!)
            .ToHashSet();

        var missing = expected
            .Where(a => !existingIds.Contains(a.CorrelationId!))
            .ToList();

        context.Set<AuditLogs>().AddRange(missing);
    }

    private static async Task CreateAuditLogsAsync(
        DbContext context,
        Transactions[] transactions,
        Wallets sender,
        CancellationToken cancellationToken)
    {
        var expected = CreateAuditLogs(transactions, sender);

        var existingIds = await context.Set<AuditLogs>()
            .Where(a => a.CorrelationId != null)
            .Select(a => a.CorrelationId!)
            .ToListAsync(cancellationToken);

        var existingSet = existingIds.ToHashSet();

        var missing = expected
            .Where(a => !existingSet.Contains(a.CorrelationId!))
            .ToList();

        await context.Set<AuditLogs>()
            .AddRangeAsync(missing, cancellationToken);
    }
}
