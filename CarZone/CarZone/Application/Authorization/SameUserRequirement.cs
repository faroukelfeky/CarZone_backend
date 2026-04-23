using Microsoft.AspNetCore.Authorization;

namespace Kawerk.Application.Authorization
{
    public class SameUserRequirement : IAuthorizationRequirement
    {
        public bool AllowAdmins { get; }
        public SameUserRequirement(bool allowAdmins = true) => AllowAdmins = allowAdmins;
    }
}
