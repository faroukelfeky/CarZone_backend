using Kawerk.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Kawerk.Application.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public Guid? UserID
        {
            get
            {
                var user = User;
                if (user?.Identity?.IsAuthenticated != true)
                    return null;
                var claim = user.FindFirst(ClaimTypes.NameIdentifier)
                            ?? user.FindFirst(JwtRegisteredClaimNames.Sub)
                            ?? user.FindFirst("sub")
                            ?? user.FindFirst("userId")
                            ?? user.FindFirst("id");
                if (claim != null && Guid.TryParse(claim.Value, out var userID))
                    return userID;
                return null;
            }
        }

        public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value ?? User?.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

        public string? Name => User?.FindFirst(ClaimTypes.Name)?.Value ?? User?.FindFirst(JwtRegisteredClaimNames.Name)?.Value;

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

        public bool IsInRole(string role)
        {
            return User?.IsInRole(role) == true;
        }
    }
}
