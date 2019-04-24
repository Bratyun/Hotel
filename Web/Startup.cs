using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Contract.Consts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.Models;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public static readonly CultureInfo[] SupportedCultures = new[]
        {
            new CultureInfo("uk"),
            new CultureInfo("en"),
        };

        /public void ConfigureServices(IServiceCollection services)
        {
            // Localization
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddSingleton<RequestLocalizationOptions>();

            // HTTPS
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 443;
            });

            //Session
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = AuthOptions.LIFETIME;
                options.Cookie.HttpOnly = false;
            });

            //Authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ReturnUrlParameter = "returnUrl";
                });

            services
                .AddMvc()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseStaticFiles();
            app.UseSession();
            app.UseCookiePolicy();
            app.UseAuthentication();

            // Localization
            RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(SupportedCultures[0]),
                SupportedCultures = SupportedCultures,
                SupportedUICultures = SupportedCultures
            };
            localizationOptions.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider());
            localizationOptions.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(async context =>
            {
                string culture = context.Session.GetString(SessionKeys.Culture);
                if (string.IsNullOrEmpty(culture))
                {
                    return null;
                }
                return new ProviderCultureResult(culture, culture);
            }));
            app.UseRequestLocalization(localizationOptions);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Room}/{action=List}");
            });
        }
    }
}
