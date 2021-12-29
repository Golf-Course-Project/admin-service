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
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());
            });

            //api versioning service
            services.AddApiVersioning();

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });

            services.AddControllers();

            // configure DI for application services
            services.Configure<AppSettings>(_configuration.GetSection("AppSettings"));

        
            services.AddScoped<IAuthRepo, AuthRepo>();
            services.AddScoped<IUsersRepo, UsersRepo>();
            services.AddScoped<ITokensRepo, TokensRepo>();      
            services.AddScoped<IStandardHelper, StandardHelper>();      
            services.AddScoped<TokenAuthorizationActionFilter>();

            // configure strongly typed settings objects
            var appSettingsSection = _configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.SecurityKey);

            // look for connection string from azure app settings first, if empty, then pull from appsettings.json
            string idenityConnectionString = string.IsNullOrEmpty(_configuration.GetConnectionString("IdentityServiceConnectionString")) ? _configuration.GetSection("AppSettings").Get<AppSettings>().IdentityServiceConnectionString : _configuration.GetConnectionString("IdentityServiceConnectionString");
            
            services.AddDbContext<IdentityDataContext>(options => options.UseSqlServer(idenityConnectionString));
            services.AddDbContext<IdentityDataContextForSp>(options => options.UseSqlServer(idenityConnectionString));
           
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {                    
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });             

            //services.AddAuthorization(options => {
            //    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin").AddRequirements(new AuthorizationRequirement()));  
            //    options.AddPolicy("Authenticated", policy => policy.Requirements.Add(new AuthorizationRequirement()));               
            //});          
            
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
