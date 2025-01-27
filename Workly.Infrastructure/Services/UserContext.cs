using Microsoft.AspNetCore.Http;
using Workly.Application.Interfaces;

namespace Workly.Infrastructure.Services
{
    public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
    {
        public int? UserId
        {
            get
            {
                var userId = httpContextAccessor.HttpContext?.Items["UserId"];
                return (int?)userId;
            }
        }
    }
}
