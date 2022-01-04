using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using AdminService.Helpers;
using AdminService.Repos;
using AdminService.Repos.Identity;
using AdminService.Misc;
using AdminService.Data;

namespace IdentityService
{
    public class Startup
    {
        public IConfiguration _configuration { get; }

        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCors(c => { c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin()); });
            services.AddHttpContextAccessor();

            //api versioning service
            services.AddApiVersioning();

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });            

            // configure DI for application services
            services.Configure<AppSettings>(_configuration.GetSection("AppSettings"));

            services.AddSingleton<IStandardHelper, StandardHelper>();
            services.AddTransient<IAuthRepo, AuthRepo>();
            services.AddTransient<IUsersRepo, UsersRepo>();
            services.AddTransient<ICodesRepo, CodesRepo>();
            services.AddTransient<ITokensRepo, TokensRepo>();
            services.AddTransient<IIdentityRepo, IdentityRepo>();

            services.AddScoped<TokenAuthorizationActionFilter>();

            // configure strongly typed settings objects
            IConfigurationSection appSettingsSection = _configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);                        
            AppSettings appSettings = appSettingsSection.Get<AppSettings>();
            
            // look for connection string from azure app settings first, if empty, then pull from appsettings.json
            string idenityConnectionString = string.IsNullOrEmpty(_configuration.GetConnectionString("IdentityServiceConnectionString")) ? _configuration.GetSection("AppSettings").Get<AppSettings>().IdentityServiceConnectionString : _configuration.GetConnectionString("IdentityServiceConnectionString");
            
            services.AddDbContext<IdentityDataContext>(options => options.UseSqlServer(idenityConnectionString));
            services.AddDbContext<IdentityDataContextForSp>(options => options.UseSqlServer(idenityConnectionString));
                       
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            // global cors policy
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
