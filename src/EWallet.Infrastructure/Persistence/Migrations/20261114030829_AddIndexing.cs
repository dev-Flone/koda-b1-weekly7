using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EWallet.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId_WalletNumber_CreatedAtUtc",
                schema: "dbo",
                table: "Wallets",
                columns: new[] { "UserId", "WalletNumber", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_WalletMutations_TransactionId_WalletId_CreatedAtUtc",
                schema: "dbo",
                table: "WalletMutations",
                columns: new[] { "TransactionId", "WalletId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName_CreatedAtUtc",
                schema: "dbo",
                table: "Users",
                columns: new[] { "UserName", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ReferenceNo_SenderWalletId_ReceiverWalletId_CreatedAtUtc",
                schema: "dbo",
                table: "Transactions",
                columns: new[] { "ReferenceNo", "SenderWalletId", "ReceiverWalletId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "TopUpRequests_TransactionId_WalletId_PamentMethodId_CreatedAtUtc",
                schema: "dbo",
                table: "TopUpRequests",
                columns: new[] { "TransactionId", "WalletId", "PaymentMethodId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_Name_IsActive_CreatedAtUtc",
                schema: "dbo",
                table: "PaymentMethods",
                columns: new[] { "Name", "IsActive", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ActorUserId_TransactionId_CreatedAtUtc",
                schema: "dbo",
                table: "AuditLogs",
                columns: new[] { "ActorUserId", "TransactionId", "CreatedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Wallets_UserId_WalletNumber_CreatedAtUtc",
                schema: "dbo",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_WalletMutations_TransactionId_WalletId_CreatedAtUtc",
                schema: "dbo",
                table: "WalletMutations");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserName_CreatedAtUtc",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_ReferenceNo_SenderWalletId_ReceiverWalletId_CreatedAtUtc",
                schema: "dbo",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "TopUpRequests_TransactionId_WalletId_PamentMethodId_CreatedAtUtc",
                schema: "dbo",
                table: "TopUpRequests");

            migrationBuilder.DropIndex(
                name: "IX_PaymentMethods_Name_IsActive_CreatedAtUtc",
                schema: "dbo",
                table: "PaymentMethods");

            migrationBuilder.DropIndex(
                name: "IX_ActorUserId_TransactionId_CreatedAtUtc",
                schema: "dbo",
                table: "AuditLogs");
        }
    }
}




