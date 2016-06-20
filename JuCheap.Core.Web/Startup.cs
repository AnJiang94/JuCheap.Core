﻿using System;
using System.Security.Claims;
using System.Security.Principal;
using JuCheap.Core.Data;
using JuCheap.Core.Interfaces;
using JuCheap.Core.Services.AppServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using JuCheap.Core.Infrastructure.Extentions;
using Microsoft.AspNetCore.Http;
namespace JuCheap.Core.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<JuCheapContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //services.AddIdentity<ApplicationUser, IdentityRole>()
            //    .AddDefaultTokenProviders();
                        
            services.AddMvc();

            // Add application services.
            services.AddScoped<IDatabaseInitService, DatabaseInitService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();

            //services.Configure<AutoMapper.IConfiguration>(cfg =>
            //{
            //    cfg = AutoMapperConfig.GetMapperConfiguration();
            //});
            //services.Configure<AutoMapper.IMapper>(mapper =>
            //{
            //    mapper = AutoMapperConfig.GetMapperConfiguration().CreateMapper();
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseStaticFiles();

            //app.UseIdentity();

            var option = new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                CookieHttpOnly = true,
                ExpireTimeSpan = TimeSpan.FromMinutes(43200),
                LoginPath = new PathString("/Home/Login"),
                LogoutPath = new PathString("/Home/Logout"),
                CookieName = ".JuCheapCore",
                CookiePath = "/",
                //DataProtectionProvider = null//如果需要做负载均衡，就需要提供一个Key
            };
            app.UseCookieAuthentication(option);

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    /// <summary>
    /// IIdentity扩展
    /// </summary>
    public static class IdentityExtention
    {
        /// <summary>
        /// 获取登录的用户ID
        /// </summary>
        /// <param name="identity">IIdentity</param>
        /// <returns></returns>
        public static int GetLoginUserId(this IIdentity identity)
        {
            var claim = (identity as ClaimsIdentity)?.FindFirst("LoginUserId");
            return claim?.Value.ToInt() ?? 0;
        }
    }
}