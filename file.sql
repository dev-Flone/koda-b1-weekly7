SELECT   *
FROM     dbo.PaymentMethods
ORDER BY Id;

SELECT   MigrationId
FROM     dbo.__EFMigrationsHistory
ORDER BY MigrationId;

SELECT name,
       is_identity
FROM   sys.columns
WHERE  object_id = OBJECT_ID('dbo.PaymentMethods')
       AND name = 'Id';

SELECT   COLUMN_NAME,
         DATA_TYPE,
         IS_NULLABLE
FROM     INFORMATION_SCHEMA.COLUMNS
WHERE    TABLE_NAME = 'RefreshTokens'
ORDER BY ORDINAL_POSITION;

SELECT   Id,
         Status,
         ExpiresAtUtc,
         PaidAtUtc
FROM     TopUpRequests
ORDER BY Id DESC;

SELECT   Id,
         Status,
         ExpiresAtUtc,
         PaidAtUtc
FROM     TopUpRequests
ORDER BY Id DESC;