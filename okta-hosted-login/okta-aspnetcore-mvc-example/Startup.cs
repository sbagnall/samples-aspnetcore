using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using okta_aspnetcore_mvc_example.Controllers;
using okta_aspnetcore_mvc_example.Services;
using Okta.AspNetCore;

namespace okta_aspnetcore_mvc_example
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

            services.AddSingleton(Configuration);
            services.Configure<ApiSettings>(Configuration.GetSection(nameof(ApiSettings)));
            services.Configure<OktaApiSettings>(Configuration.GetSection(nameof(OktaApiSettings)));

            var oktaMvcOptions = new OktaMvcOptions();
            Configuration.GetSection("Okta").Bind(oktaMvcOptions);
            oktaMvcOptions.Scope = new List<string> {"openid", "profile", "email", "custom", "api" }; 
            oktaMvcOptions.GetClaimsFromUserInfoEndpoint = false;

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddOktaMvc(oktaMvcOptions);
            
            services.AddAuthorization(options  =>
            {
                options.AddPolicy("AdminGroup", policy =>
                    policy.RequireClaim("admin_group"));
            });

            services.AddHttpContextAccessor();
            //services.AddHttpClient();

            services.AddSingleton<ITokenService, OktaTokenService>();
            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
