using ForumSiteCore.Business.Responses;
using ForumSiteCore.Business.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSiteCore.BlazorWasm.Client.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(LoginVM loginModel);
        Task Logout();
        Task<RegisterResponse> Register(RegisterVM registerModel);
    }
}
