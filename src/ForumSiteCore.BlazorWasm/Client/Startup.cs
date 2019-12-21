using Blazored.LocalStorage;
using ForumSiteCore.BlazorWasm.Client.Interfaces;
using ForumSiteCore.BlazorWasm.Client.Services;
using Ganss.XSS;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace ForumSiteCore.BlazorWasm.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHtmlSanitizer, HtmlSanitizer>(x =>
            {
                var htmlSanitizer = new HtmlSanitizer();
                htmlSanitizer.AllowedAttributes.Add("class");
                return htmlSanitizer;
            });
            services.AddSingleton<TimeagoService>();
            services.AddBlazoredLocalStorage();
            services.AddAuthorizationCore();
            services.AddScoped<ApiAuthenticationStateProvider>();
            services.AddScoped<IAuthService, AuthService>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
