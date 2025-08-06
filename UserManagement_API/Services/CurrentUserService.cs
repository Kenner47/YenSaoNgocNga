using System.Security.Claims;
using UserManagement_API.Helpers;
using UserManagement_API.Services.IService;

namespace UserManagement_API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetCurrentUserId()
        {
            var identity = _httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            var extractedId = ClaimsHelper.GetCurrentUserId(identity);
            return string.IsNullOrEmpty(extractedId) ? -1 : int.Parse(extractedId);
        }

        public string? GetCurrentUserRole()
        {
            var identity = _httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            return ClaimsHelper.GetCurrentUserRole(identity);
        }

        public string? GetCurrentUserEmail()
        {
            var identity = _httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            return identity?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        }
    }
}