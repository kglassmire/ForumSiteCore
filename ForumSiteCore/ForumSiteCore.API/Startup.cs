using System;
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
            services.AddCors();
            services.AddMvc();
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient(typeof(ForumService));
            services.AddTransient(typeof(PostService));
            services.AddTransient(typeof(CommentService));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerUi(typeof(Startup).GetTypeInfo().Assembly, new SwaggerUiSettings());
            }

            app.UseCors(builder => 
                builder.WithOrigins(new []{ "http://localhost:4200" } ) // remember to use an origin here, not a url -- "http://localhost:4200" -- origin, "http://localhost:4200/ -- url
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                );

            app.UseMvc();
        }
    }
}
