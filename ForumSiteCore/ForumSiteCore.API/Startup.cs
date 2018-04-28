﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForumSiteCore.Business.Services;
using ForumSiteCore.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ForumSiteCore.Business;
using NSwag.AspNetCore;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using ForumSiteCore.DAL.Models;
using CacheManager;
using CacheManager.Core;
using Serilog;
using Microsoft.AspNetCore.Http;
using ForumSiteCore.Business.Interfaces;

namespace ForumSiteCore.API
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
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                //.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.RollingFile(@".\Logs\log-{Date}.txt")
                .CreateLogger();

            services.AddCors();
            services.AddMvc();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            services.AddScoped(typeof(ForumService));
            services.AddScoped(typeof(PostService));
            services.AddScoped(typeof(CommentService));
            services.AddScoped(typeof(UserActivitiesService));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserAccessor<Int64>, UserAccessor>();
            services.AddCacheManagerConfiguration(cfg => cfg.WithMicrosoftMemoryCacheHandle().And.WithMicrosoftLogging(f => f.AddSerilog()));
            services.AddCacheManager();
            ConfigureIdentity(services);
            ConfigureCookieSettings(services);
        }

        private void ConfigureCookieSettings(IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.LoginPath = String.Empty;
                options.Events.OnRedirectToLogin = (context) =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
                //options.Cookie.Expiration = TimeSpan.FromDays(150);
                //options.LoginPath = "/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
                //options.LogoutPath = "/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                //options.AccessDeniedPath = "/Account/AccessDenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                //options.SlidingExpiration = true;
            });
        }

        private void ConfigureIdentity(IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                // configure stuff here
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerUi(typeof(Startup).GetTypeInfo().Assembly);
            }

            app.UseAuthentication();

            app.UseCors(builder => 
                builder//.WithOrigins(new []{ "http://localhost:4200", "http://localhost:5000" } ) // remember to use an origin here, not a url -- "http://localhost:4200" -- origin, "http://localhost:4200/ -- url
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                );

            app.UseMvc();

            
        }
    }
}
