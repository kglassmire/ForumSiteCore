﻿using System;
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
using ForumSiteCore.DAL;
using FluentValidation.AspNetCore;
using ForumSiteCore.DAL.Models;
using ForumSiteCore.Web.Areas.Identity.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using ForumSiteCore.Business.Services;
using ForumSiteCore.Business.Interfaces;
using ForumSiteCore.Web.Controllers;
using CacheManager.Core;
using ForumSiteCore.Business;

namespace ForumSiteCore.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AutoMapperConfiguration.RegisterMappings();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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
            services.AddScoped(typeof(ForumController));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserAccessor<Int64>, UserAccessor>();
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
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<LoginVMValidator>());

            ConfigureIdentity(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
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
