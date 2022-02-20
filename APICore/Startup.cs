using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog.Extensions.Logging;
using NLog.Web;
using Microsoft.Extensions.DependencyInjection.Extensions;
using APIAPI;
using System.IO;
using System.Text;
using AutoMapper;
using API.Helper;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Reflection;
using API.WebAPIs.Filters;
using API.Data.DBEntities;
using Microsoft.EntityFrameworkCore;
using API.Implementation.Configuration;

namespace API
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


            #region dbcontext
            new DependencyManager().InjectDependencies(services);
            var mysqlConString = "";

            if (mysqlConString == null)
                mysqlConString = Configuration.GetConnectionString("DBConnection");
              
          services.AddDbContext<APIDbContext>(options => options.UseSqlServer( Configuration.GetConnectionString("DBConnection")));

            #endregion



            services.AddDbContext<APIDbContext>();

            services.AddControllersWithViews();

            // Add the Services Here
          

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                 
                c.OperationFilter<AuthorizeOperationFilter>();
            });
            
            services.AddApiVersioning(x =>
            { 
                x.ReportApiVersions = true;
                x.ApiVersionReader = new HeaderApiVersionReader("api-version");
                x.DefaultApiVersion = new ApiVersion(1, 0);
                x.AssumeDefaultVersionWhenUnspecified = true;
            });
            services.AddHttpContextAccessor();
            services.TryAddSingleton<Microsoft.AspNetCore.Mvc.Infrastructure.IActionContextAccessor, Microsoft.AspNetCore.Mvc.Infrastructure.ActionContextAccessor>();
        }

       
        public void Configure(IApplicationBuilder app, IHostEnvironment env,  ILoggerFactory loggerFactory)
      {
            
            {
                bool isLogEnabled = true;  
 

                if (isLogEnabled)
                    app.UseMiddleware<RequestResponseLoggingMiddleware>();  

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                app.UseSwagger();
                 
                app.UseSwaggerUI(c =>
                {
                   
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API API - v1");
                });
                app.UseHttpsRedirection();

                app.UseRouting();

                

                app.UseAuthentication();
                app.UseAuthorization();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
                });

                 

            }
        }
    }
}
