namespace EWallet.Application.DTOs.Admin;

public record class AdminDashboardResponse(
    int TotalUsers,
    int ActiveUsers,
    int SuspendedUsers,
    List<RecentUser> RecentUsers
);
