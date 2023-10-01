using Microsoft.AspNetCore.Authorization;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace MiniApp1.API.Requirements
{
    public class BirthDateRequirement : IAuthorizationRequirement
    {
        public int Age { get; set; }

        public BirthDateRequirement(int age)
        {
            Age = age;
        }
    }

    public class BirthDateRequirementHandler : AuthorizationHandler<BirthDateRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BirthDateRequirement requirement)
        {
            var userBirthDate = context.User.FindFirst("BirthDate");
            if (userBirthDate is null)
            {
                context.Fail();
                return Task.CompletedTask;
            }
            var today = DateTime.Now;
            var age = today.Year - Convert.ToDateTime(userBirthDate.Value).Year;

            if(age >= requirement.Age) context.Succeed(requirement);
            else context.Fail();
            return Task.CompletedTask;
        }
    }
}
