namespace UserManagement_API.Services.IService
{
    public interface ICurrentUserService
    {
        int GetCurrentUserId();
        string? GetCurrentUserRole();
        string? GetCurrentUserEmail();
    }
}
