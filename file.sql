-- Semua transaksi yang melibatkan wallet 21 dan 22
SELECT   Id,
         ReferenceNo,
         SenderWalletId,
         ReceiverWalletId,
         Amount,
         Type,
         Status,
         CreatedAtUtc
FROM     dbo.Transactions
WHERE    SenderWalletId IN (21, 22)
         OR ReceiverWalletId IN (21, 22)
ORDER BY CreatedAtUtc, Id;

-- Semua mutasi pada wallet 21 dan 22
SELECT   Id,
         TransactionId,
         WalletId,
         Direction,
         Amount,
         BalanceBefore,
         BalanceAfter,
         CreatedAtUtc
FROM     dbo.WalletMutations
WHERE    WalletId IN (21, 22)
ORDER BY WalletId, CreatedAtUtc, Id;

-- Top-up yang terkait dengan wallet 21 dan 22
SELECT   Id,
         TransactionId,
         WalletId,
         Amount,
         Status,
         CreatedAtUtc,
         PaidAtUtc
FROM     dbo.TopUpRequests
WHERE    WalletId IN (21, 22)
ORDER BY CreatedAtUtc, Id;

-- CHECK
-- Seed transactions
SELECT   ReferenceNo,
         COUNT(*) AS Total
FROM     Transactions
WHERE    ReferenceNo LIKE 'SEED-%'
GROUP BY ReferenceNo
ORDER BY ReferenceNo;

-- Seed top-up requests
SELECT   t.ReferenceNo,
         COUNT(*) AS Total
FROM     TopUpRequests AS tur
         INNER JOIN
         Transactions AS t
         ON t.Id = tur.TransactionId
WHERE    t.ReferenceNo LIKE 'SEED-%'
GROUP BY t.ReferenceNo
ORDER BY t.ReferenceNo;

-- Seed wallet mutations
SELECT   t.ReferenceNo,
         COUNT(*) AS Total
FROM     WalletMutations AS wm
         INNER JOIN
         Transactions AS t
         ON t.Id = wm.TransactionId
WHERE    t.ReferenceNo LIKE 'SEED-%'
GROUP BY t.ReferenceNo
ORDER BY t.ReferenceNo;

-- Seed audit logs
SELECT   CorrelationId,
         COUNT(*) AS Total
FROM     AuditLogs
WHERE    CorrelationId LIKE 'SEED-%'
GROUP BY CorrelationId
ORDER BY CorrelationId;

-- CHECK AGAIN
SELECT   w.Id AS WalletId,
         w.WalletNumber,
         w.Balance,
         SUM(CASE WHEN wm.Direction = 'IN' THEN wm.Amount WHEN wm.Direction = 'OUT' THEN -wm.Amount ELSE 0 END) AS SeedNetMutation
FROM     Wallets AS w
         INNER JOIN
         WalletMutations AS wm
         ON wm.WalletId = w.Id
         INNER JOIN
         Transactions AS t
         ON t.Id = wm.TransactionId
WHERE    t.ReferenceNo LIKE 'SEED-%'
GROUP BY w.Id, w.WalletNumber, w.Balance
ORDER BY w.Id;

-- AUDIT CHECK
-- 1. Transaction without valid receiver wallet
SELECT t.Id,
       t.ReferenceNo,
       t.ReceiverWalletId
FROM   Transactions AS t
       LEFT OUTER JOIN
       Wallets AS w
       ON w.Id = t.ReceiverWalletId
WHERE  w.Id IS NULL;

-- 2. Transfer without valid sender wallet
SELECT t.Id,
       t.ReferenceNo,
       t.SenderWalletId
FROM   Transactions AS t
       LEFT OUTER JOIN
       Wallets AS w
       ON w.Id = t.SenderWalletId
WHERE  t.Type = 'TRANSFER'
       AND w.Id IS NULL;

-- 3. Top-up without TopUpRequest
SELECT t.Id,
       t.ReferenceNo
FROM   Transactions AS t
       LEFT OUTER JOIN
       TopUpRequests AS tur
       ON tur.TransactionId = t.Id
WHERE  t.Type = 'TOPUP'
       AND tur.Id IS NULL;

-- 4. TopUpRequest without matching transaction
SELECT tur.Id,
       tur.TransactionId
FROM   TopUpRequests AS tur
       LEFT OUTER JOIN
       Transactions AS t
       ON t.Id = tur.TransactionId
WHERE  t.Id IS NULL;

-- 5. WalletMutation without matching transaction
SELECT wm.Id,
       wm.TransactionId
FROM   WalletMutations AS wm
       LEFT OUTER JOIN
       Transactions AS t
       ON t.Id = wm.TransactionId
WHERE  t.Id IS NULL;

-- 6. AuditLog without matching transaction
SELECT al.Id,
       al.TransactionId
FROM   AuditLogs AS al
       LEFT OUTER JOIN
       Transactions AS t
       ON t.Id = al.TransactionId
WHERE  al.TransactionId IS NOT NULL
       AND t.Id IS NULL;

--   AUDIT TOPUP_REQUEST
SELECT t.Id AS TransactionId,
       t.ReferenceNo,
       t.Amount AS TransactionAmount,
       tur.Amount AS RequestAmount,
       t.ReceiverWalletId,
       tur.WalletId
FROM   TopUpRequests AS tur
       INNER JOIN
       Transactions AS t
       ON t.Id = tur.TransactionId
WHERE  t.Type <> 'TOPUP'
       OR t.Amount <> tur.Amount
       OR t.ReceiverWalletId <> tur.WalletId;

-- AUDIT JUMLAH MUTASI TRANSAKSI BERHASIL
SELECT   t.Id,
         t.ReferenceNo,
         t.Type,
         t.Status,
         COUNT(wm.Id) AS MutationCount
FROM     Transactions AS t
         LEFT OUTER JOIN
         WalletMutations AS wm
         ON wm.TransactionId = t.Id
WHERE    t.Status = 'SUCCESS'
GROUP BY t.Id, t.ReferenceNo, t.Type, t.Status
HAVING   (t.Type = 'TOPUP'
          AND COUNT(wm.Id) <> 1)
         OR (t.Type = 'TRANSFER'
             AND COUNT(wm.Id) <> 2);

-- AUDIT KONSISTENSI SALDO DENGAN MUTASI
SELECT   w.Id AS WalletId,
         w.WalletNumber,
         w.Balance AS CurrentBalance,
         COALESCE (SUM(CASE WHEN wm.Direction = 'IN' THEN wm.Amount WHEN wm.Direction = 'OUT' THEN -wm.Amount ELSE 0 END), 0) AS CalculatedBalance
FROM     Wallets AS w
         LEFT OUTER JOIN
         WalletMutations AS wm
         ON wm.WalletId = w.Id
GROUP BY w.Id, w.WalletNumber, w.Balance
HAVING   w.Balance <> COALESCE (SUM(CASE WHEN wm.Direction = 'IN' THEN wm.Amount WHEN wm.Direction = 'OUT' THEN -wm.Amount ELSE 0 END), 0);

-- AUDIT NILAI SALDO SEBELUM DAN SESUDAH
SELECT Id,
       TransactionId,
       WalletId,
       Direction,
       Amount,
       BalanceBefore,
       BalanceAfter
FROM   WalletMutations
WHERE  (Direction = 'IN'
        AND BalanceAfter <> BalanceBefore + Amount)
       OR (Direction = 'OUT'
           AND BalanceAfter <> BalanceBefore - Amount);