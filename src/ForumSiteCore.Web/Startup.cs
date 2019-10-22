using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ForumSiteCore.DAL;
using ForumSiteCore.DAL.Models;
using ForumSiteCore.Web.Areas.Identity.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using ForumSiteCore.Business.Services;
using ForumSiteCore.Business.Interfaces;
using ForumSiteCore.Web.Controllers;
using CacheManager.Core;
using ForumSiteCore.Business;
using NSwag.AspNetCore;
using System.IO;
using FluentValidation.AspNetCore;
using ForumSiteCore.Business.Validators;
using Microsoft.AspNetCore.SpaServices.Webpack;
using AutoMapper;

namespace ForumSiteCore.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddScoped(typeof(ForumService));
            services.AddScoped(typeof(PostService));
            services.AddScoped(typeof(CommentService));
            services.AddScoped(typeof(UserActivitiesService));
            services.AddScoped(typeof(ForumApiController));
            services.AddScoped(typeof(PostApiController));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserAccessor<Int64>, UserAccessor>();
            services.AddScoped<IEmailSender, EmailSender>();

            services.AddCacheManagerConfiguration(cfg => cfg
                .WithMicrosoftMemoryCacheHandle()
                .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(60)));
                //.And.WithMicrosoftLogging(f => f.AddSerilog()));
            services.AddCacheManager();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("ForumSiteCore.DAL")));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()                
                .AddDefaultTokenProviders();

            services.AddAutoMapper(typeof(AutoMapperConfigProfile));

            services.AddResponseCaching();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<LoginVMValidator>());

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddSwaggerDocument();

            ConfigureIdentity(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseOpenApi();
                app.UseSwaggerUi3();
                app.UseDeveloperExceptionPage();

                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    ProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "ClientApp"),
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCaching();
            app.UseStatusCodePagesWithReExecute("/Error/", "?statusCode={0}");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=ForumWeb}/{action=Index}/{id?}"
                );
            });            
        }

        private void ConfigureIdentity(IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                // configure stuff here                
            });
        }
    }
}
