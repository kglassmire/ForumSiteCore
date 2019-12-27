using ForumSiteCore.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Text;

namespace ForumSiteCore.Business.Services
{
    public class UserAccessor : IUserAccessor<long>
    {
        IHttpContextAccessor _httpContextAccessor;
        public UserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal User { get => _httpContextAccessor.HttpContext.User; }

        public long UserId
        {
            get
            {
                if (User == null)
                    throw new ArgumentNullException(nameof(User));

                return Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }

        }
        public string UserName { get => User?.Identity.Name; }
    }
}
