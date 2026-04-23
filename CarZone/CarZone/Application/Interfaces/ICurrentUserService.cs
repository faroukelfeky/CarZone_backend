using System.Security.Claims;

namespace Kawerk.Application.Interfaces
{
    public interface ICurrentUserService
    {
        ClaimsPrincipal? User { get; }
        Guid? UserID { get; }
        string? Email { get; }
        string? Name { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(string role);
    }
}
