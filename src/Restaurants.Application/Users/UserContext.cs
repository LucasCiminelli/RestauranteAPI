using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Users
{
    public interface IUserContext
    {
        CurrentUser? GetCurrentUser();
    }

    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserContext(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public CurrentUser GetCurrentUser()
        {
            var user = _contextAccessor?.HttpContext?.User;

            if (user == null)
            {

                return null!;
            }

            if (user.Identity == null || !user.Identity.IsAuthenticated)
            {
                return null!;
            }

            var userId = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
            var emailAddress = user.FindFirst(c => c.Type == ClaimTypes.Email)!.Value;
            var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(r => r.Value);
            var nationality = user.FindFirst(c => c.Type == "Nationality")?.Value ?? string.Empty;
            var dateOfBithString = user.FindFirst(c => c.Type == "DateOfBirth")?.Value;
            var dateOfBIrth = dateOfBithString == null
                ? (DateOnly?)null
                : DateOnly.ParseExact(dateOfBithString, "yyyy-MM-dd");


            return new CurrentUser(userId, emailAddress, roles, nationality,dateOfBIrth);

        }
    }
}
