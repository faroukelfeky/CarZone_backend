using Microsoft.AspNetCore.Authorization;

namespace CarZone.Application.Authorization
{
    public class SameUserRequirement : IAuthorizationRequirement
    {
        public bool AllowAdmins { get; }
        public SameUserRequirement(bool allowAdmins = true) => AllowAdmins = allowAdmins;
    }
}