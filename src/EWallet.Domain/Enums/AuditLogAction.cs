
namespace EWallet.Domain.Enums;

public enum AuditLogAction
{
    TOPUP_REQUESTED,
    TOPUP_PAID,
    TRANSFER_CREATED,
    TRANSFER_REJECTED,
    USER_SUSPENDED,
    WALLET_STATUS_CHANGED
}
