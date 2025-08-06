namespace UserManagement_API.Models.DTOs
{
    public class UserStatisticsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public UsersByRoleDto UsersByRole { get; set; } = new();
        public UsersByGenderDto UsersByGender { get; set; } = new();
        public RecentUsersDto RecentUsers { get; set; } = new();
    }

    public class UsersByRoleDto
    {
        public int AdminCount { get; set; }
        public int EmployeeCount { get; set; }
        public int UserCount { get; set; }
    }

    public class UsersByGenderDto
    {
        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }
        public int OtherCount { get; set; }
    }

    public class RecentUsersDto
    {
        public int TodayRegistrations { get; set; }
        public int ThisWeekRegistrations { get; set; }
        public int ThisMonthRegistrations { get; set; }
    }
}