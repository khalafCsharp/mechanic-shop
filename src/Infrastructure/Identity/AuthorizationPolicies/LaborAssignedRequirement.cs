using System.Security.Claims;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Domain.Features.Employees;

namespace Infrastructure.Identity.AuthorizationPolicies
{
    public class LaborAssignedRequirement : IAuthorizationRequirement;


    public class LaborAssignedRequirementHandler : AuthorizationHandler<LaborAssignedRequirement>
    {
        private readonly IAppDbContext dbContext;
        private readonly IHttpContextAccessor httpContextAccessor;

        public LaborAssignedRequirementHandler(IAppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.dbContext = context;
            this.httpContextAccessor = httpContextAccessor;
        }

        protected override  async Task HandleRequirementAsync(AuthorizationHandlerContext authContext, LaborAssignedRequirement requirement)
        {
            string? userId = authContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(string.IsNullOrEmpty(userId))
            {
                authContext.Fail();
                return ;
            }
           
            var workOrderIdString = httpContextAccessor.HttpContext?.Request.RouteValues["WorkOrderId"]?.ToString();

            if (!Guid.TryParse(workOrderIdString, out var workOrderId))
            {
                authContext.Fail();
                return;
            }

            var isItsLabor = await dbContext.WorkOrders.AnyAsync(a => a.Id == workOrderId && a.LaborId == Guid.Parse(userId));

            if (isItsLabor)
            {
                authContext.Succeed(requirement);
                return;
            }
            if (authContext.User.IsInRole(nameof(Role.Manager)))
            {
                authContext.Succeed(requirement);
                return;
            }

            authContext.Fail();
        }
    }

  
}