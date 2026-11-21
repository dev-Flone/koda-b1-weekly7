using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EWallet.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePaymentMethodFixedIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                    """
            -- 1. Lepaskan foreign key sementara.
            ALTER TABLE dbo.TopUpRequests
            DROP CONSTRAINT FK_TopUpRequests_PaymentMethods_PaymentMethodId;

            -- 2. Periksa kondisi data payment method.
            DECLARE @PaymentMethodCount int;

            SELECT @PaymentMethodCount = COUNT(*)
            FROM dbo.PaymentMethods;

            -- Jika tabel tidak kosong, validasi data lama.
            IF @PaymentMethodCount > 0
            BEGIN
                IF EXISTS (
                    SELECT 1
                    FROM dbo.PaymentMethods
                    WHERE Code NOT IN ('BANK_TRANSFER', 'QRIS')
                )
                BEGIN
                    THROW 51000, 'Unexpected payment method found.', 1;
                END;

                IF @PaymentMethodCount <> 2
                BEGIN
                    THROW 51001, 'Expected payment methods are missing.', 1;
                END;

                -- Pastikan kedua payment method memang tersedia.
                IF NOT EXISTS (
                    SELECT 1
                    FROM dbo.PaymentMethods
                    WHERE Code = 'BANK_TRANSFER'
                )
                OR NOT EXISTS (
                    SELECT 1
                    FROM dbo.PaymentMethods
                    WHERE Code = 'QRIS'
                )
                BEGIN
                    THROW 51001, 'Expected payment methods are missing.', 1;
                END;

                -- 3. Pindahkan referensi TopUpRequest.
                UPDATE tr
                SET PaymentMethodId =
                    CASE pm.Code
                        WHEN 'BANK_TRANSFER' THEN 0
                        WHEN 'QRIS' THEN 1
                    END
                FROM dbo.TopUpRequests tr
                INNER JOIN dbo.PaymentMethods pm
                    ON tr.PaymentMethodId = pm.Id;
            END
            ELSE
            BEGIN
                -- Database fresh seharusnya belum memiliki TopUpRequest.
                IF EXISTS (
                    SELECT 1
                    FROM dbo.TopUpRequests
                )
                BEGIN
                    THROW 51002, 'TopUpRequests exist without payment methods.', 1;
                END;
            END;

            -- 4. Hapus tabel lama yang menggunakan IDENTITY.
            DROP TABLE dbo.PaymentMethods;

            -- 5. Buat ulang tabel tanpa IDENTITY.
            CREATE TABLE dbo.PaymentMethods
            (
                Id int NOT NULL,
                Code nvarchar(30) NOT NULL,
                Name nvarchar(100) NOT NULL,
                ProviderName nvarchar(100) NULL,
                IsActive bit NOT NULL,
                CreatedAtUtc datetime2 NOT NULL,

                CONSTRAINT PK_PaymentMethods
                    PRIMARY KEY (Id)
            );

            -- 6. Buat unique index.
            CREATE UNIQUE INDEX IX_PaymentMethods_Code
            ON dbo.PaymentMethods(Code);

            -- 7. Masukkan fixed payment methods.
            INSERT INTO dbo.PaymentMethods
            (
                Id,
                Code,
                Name,
                ProviderName,
                IsActive,
                CreatedAtUtc
            )
            VALUES
            (
                0,
                'BANK_TRANSFER',
                'Bank Transfer',
                'Bank',
                1,
                '2026-01-01T00:00:00'
            ),
            (
                1,
                'QRIS',
                'QRIS',
                'QRIS',
                1,
                '2026-01-01T00:00:00'
            );

            -- 8. Pasang kembali foreign key.
            ALTER TABLE dbo.TopUpRequests
            WITH CHECK
            ADD CONSTRAINT FK_TopUpRequests_PaymentMethods_PaymentMethodId
            FOREIGN KEY (PaymentMethodId)
            REFERENCES dbo.PaymentMethods(Id)
            ON DELETE NO ACTION;

            ALTER TABLE dbo.TopUpRequests
            CHECK CONSTRAINT FK_TopUpRequests_PaymentMethods_PaymentMethodId;
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                    """
            -- 1. Lepaskan foreign key.
            ALTER TABLE dbo.TopUpRequests
            DROP CONSTRAINT FK_TopUpRequests_PaymentMethods_PaymentMethodId;

            -- 2. Kembalikan referensi ID lama.
            UPDATE dbo.TopUpRequests
            SET PaymentMethodId =
                CASE PaymentMethodId
                    WHEN 0 THEN 5
                    WHEN 1 THEN 6
                END
            WHERE PaymentMethodId IN (0, 1);

            -- 3. Hapus tabel fixed ID.
            DROP TABLE dbo.PaymentMethods;

            -- 4. Buat kembali tabel dengan IDENTITY.
            CREATE TABLE dbo.PaymentMethods
            (
                Id int IDENTITY(1,1) NOT NULL,
                Code nvarchar(30) NOT NULL,
                Name nvarchar(100) NOT NULL,
                ProviderName nvarchar(100) NULL,
                IsActive bit NOT NULL,
                CreatedAtUtc datetime2 NOT NULL,

                CONSTRAINT PK_PaymentMethods
                    PRIMARY KEY (Id)
            );

            -- 5. Kembalikan data lama dengan ID 5 dan 6.
            SET IDENTITY_INSERT dbo.PaymentMethods ON;

            INSERT INTO dbo.PaymentMethods
            (
                Id,
                Code,
                Name,
                ProviderName,
                IsActive,
                CreatedAtUtc
            )
            VALUES
            (
                5,
                'BANK_TRANSFER',
                'Bank Transfer',
                'Bank',
                1,
                '2026-01-01T00:00:00'
            ),
            (
                6,
                'QRIS',
                'QRIS',
                'QRIS',
                1,
                '2026-01-01T00:00:00'
            );

            SET IDENTITY_INSERT dbo.PaymentMethods OFF;

            -- 6. Kembalikan unique index.
            CREATE UNIQUE INDEX IX_PaymentMethods_Code
            ON dbo.PaymentMethods(Code);

            -- 7. Pasang kembali foreign key.
            ALTER TABLE dbo.TopUpRequests
            WITH CHECK
            ADD CONSTRAINT FK_TopUpRequests_PaymentMethods_PaymentMethodId
            FOREIGN KEY (PaymentMethodId)
            REFERENCES dbo.PaymentMethods(Id)
            ON DELETE NO ACTION;

            ALTER TABLE dbo.TopUpRequests
            CHECK CONSTRAINT FK_TopUpRequests_PaymentMethods_PaymentMethodId;
            """);
        }
    }
}




