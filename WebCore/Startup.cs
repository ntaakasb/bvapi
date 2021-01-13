using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebCore.Commands;
using WebCore.Infrastructure.BusinessObjects;
using WebCore.Infrastructure.Middlewares;
using WebCore.Models;
using WebCore.Queries;
using WebCore.Services;
using IAuthenticationAppService = WebCore.Services.IAuthenticationAppService;

namespace WebCore
{

    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Configuration = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            ConfigureDbContext(services);
            ConfigureSecurity(services);
            RegisterCQProcessors(services);
            RegisterServices(services);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            ResolveAllTypes(services, typeof(ICommandHandler<>));
            ResolveAllTypes(services, typeof(IQueryHandler<,>));
            services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            ConfigureAuth(app);
            app.UseHttpsRedirection();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Default}/{action=Index}/{id?}");
            });
        }

        private void ConfigureSecurity(IServiceCollection services)
        {
            // add azure authentication
            services.Configure<AuthenticationOptions>(Configuration.GetSection("Authentication"));

            // add jwt token to security
            services.Configure<TokenOptions>(Configuration.GetSection("TokenOptions"));
            var settings = Configuration.GetSection("TokenOptions").Get<TokenOptions>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.Secret)),
                        ValidateIssuer = true,
                        ValidIssuer = settings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = settings.Audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        RequireExpirationTime = true
                    };
                });
        }

        protected virtual void ConfigureDbContext(IServiceCollection services)
        {
            services.AddDbContextPool<DwhContext>(options => options.UseOracle(Configuration.GetConnectionString("DwhContext")));
        }

        private void RegisterCQProcessors(IServiceCollection services)
        {
            services.AddScoped<IQueryProcessor, QueryProcessor>();
            services.AddScoped<ICommandProcessor, CommandProcessor>();
        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseAuthentication();
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<DbContext, DwhContext>();
            services.AddScoped<IAuthenticationAppService, AuthenticationAppService>();
        }

        public static void ResolveAllTypes(IServiceCollection services, Type handlerInterface)
        {
            var handlers = typeof(Startup).Assembly.GetTypes()
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterface));

            foreach (var handler in handlers)
            {
                foreach (var queryHandler in handler.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterface))
                {
                    services.AddScoped(queryHandler, handler);
                }
            }
        }
    }

}
