using AutoMapper;
using CacheManager.Core;
using ForumSiteCore.Business;
using ForumSiteCore.Business.Interfaces;
using ForumSiteCore.Business.Services;
using ForumSiteCore.DAL;
using ForumSiteCore.DAL.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace ForumSiteCore.BlazorWasm.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCacheManagerConfiguration(cfg => cfg
                .WithMicrosoftMemoryCacheHandle()
                .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(60)));
            //.And.WithMicrosoftLogging(f => f.AddSerilog()));
            services.AddCacheManager();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("ForumSiteCore.DAL")));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["JwtIssuer"],
                        ValidAudience = Configuration["JwtAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSecurityKey"]))
                    };
                });

            services.AddScoped(typeof(ForumService));
            services.AddScoped(typeof(PostService));
            services.AddScoped(typeof(CommentService));
            services.AddScoped(typeof(CurrentUserActivitiesService));
            services.AddScoped(typeof(UserProfileService));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserAccessor<Int64>, UserAccessor>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddAutoMapper(typeof(AutoMapperConfigProfile));

            services.AddResponseCaching();

            services.AddMvc();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBlazorDebugging();
            }

            app.UseStaticFiles();
            app.UseClientSideBlazorFiles<Client.Startup>();

            app.UseRouting();
            app.UseResponseCaching();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapFallbackToClientSideBlazor<Client.Startup>("index.html");
                //  to allow for server-side prerendering of index page (hosting blazor "app" element):
                //  by default it is MapFallbackToClientSideBlazor
                //  changed this to MapFallbackToPage on server app
                //  and an http client needs to be added with base uri like 
                //             
                //  services.AddScoped(s =>
                //  {
                //      return new HttpClient
                //      {
                //          BaseAddress = new Uri("https://localhost:44390")
                //      };
                //  });
                //  https://github.com/danroth27/BlazorWebAssemblyWithPrerendering/blob/master/BlazorWebAssemblyWithPrerendering.Server/Startup.cs
                // THIS IS THE LINE --> endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
