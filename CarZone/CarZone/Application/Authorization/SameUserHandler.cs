using Kawerk.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Kawerk.Application.Authorization
{
    public class SameUserHandler : AuthorizationHandler<SameUserRequirement, Guid>
    {
        private readonly ICurrentUserService _currentUserService;
        public SameUserHandler(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserRequirement requirement, Guid resource)
        {
            
            // Admin bypass
            if (requirement.AllowAdmins && _currentUserService.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Try common claim names for user id
            var ClaimID = _currentUserService.UserID;

            if (ClaimID == resource)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
