using Ganss.XSS;
using Microsoft.AspNetCore.Components.Builder;
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
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
