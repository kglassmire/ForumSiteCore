using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace ForumSiteCore.Business.Interfaces
{
    public interface IUserAccessor<T>
    {
        ClaimsPrincipal User { get; }
        T UserId { get; }
        String UserName { get; }
    }
}
