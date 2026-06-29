using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Data.Services
{
    public class CurrentUserService
    {
        private readonly IHttpContextAccessor _accessor;

        public CurrentUserService(IHttpContextAccessor accessor) => _accessor = accessor;

        public string Name => _accessor.HttpContext?.User?.Identity?.Name ?? "system";
        public string Id => _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
    }
}
