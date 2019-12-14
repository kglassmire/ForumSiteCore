using AutoMapper;
using CacheManager.Core;
using ForumSiteCore.Business;
using ForumSiteCore.Business.Interfaces;
using ForumSiteCore.Business.Services;
using ForumSiteCore.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

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

            services.AddScoped(typeof(ForumService));
            services.AddScoped(typeof(PostService));
            services.AddScoped(typeof(CommentService));
            services.AddScoped(typeof(UserActivitiesService));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserAccessor<Int64>, UserAccessor>();
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapFallbackToClientSideBlazor<Client.Startup>("index.html");
            });
        }
    }
}
