using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using API.DAL;
using API.Timers;
using Contract.Consts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MySqlLib;

namespace API
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("config.json");
            // создаем конфигурацию
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //connect to database
            string connectionString = Configuration["ConnectionStrings:Default"];
            services.AddTransient((serviceProvider) =>
            {
                var mysql = new MySqlData();
                mysql.SetConnection(connectionString);

                CheckTimerStart(mysql);
                RoomTimerStart(mysql);

                return mysql;
            });
            services.AddTransient<ApiCheck>();
            services.AddTransient<ApiHotel>();
            services.AddTransient<ApiRequest>();
            services.AddTransient<ApiRoom>();
            services.AddTransient<ApiUser>();
            
            
            // JWT
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    byte[] symmetricKey = Convert.FromBase64String(AuthOptionsPrivate.KEY);
                    //options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // укзывает, будет ли валидироваться издатель при валидации токена
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.ISSUER,

                        // будет ли валидироваться время существования
                        ValidateLifetime = true,

                        // установка ключа безопасности
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(symmetricKey),

                        //RoleClaimType = "role",
                        RequireExpirationTime = true,
                        ValidateAudience = false
                    };
                });
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

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        /// <summary>
        /// Start timer which call method of verify valid of check status
        /// </summary>
        private static void CheckTimerStart(MySqlData mySqlData)
        {
            Timer checkTimer = new Timer(120000);
            TimeControl timeControl = new TimeControl(new ApiRoom(mySqlData), new ApiCheck(mySqlData));
            checkTimer.Elapsed += timeControl.CheckTimeOver;
            checkTimer.AutoReset = true;
            checkTimer.Enabled = true;
        }

        /// <summary>
        /// Start timer which call method of verify valid of room status
        /// </summary>
        private static void RoomTimerStart(MySqlData mySqlData)
        {
            Timer roomTimer = new Timer(120000);
            TimeControl timeControl = new TimeControl(new ApiRoom(mySqlData), new ApiCheck(mySqlData));
            roomTimer.Elapsed += timeControl.RoomTimeOver;
            roomTimer.AutoReset = true;
            roomTimer.Enabled = true;
        }
    }

    public static class ProtocolExtensions
    {
        public static IApplicationBuilder UseHttpsOnly(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpsOnlyMiddleware>();
        }
    }

    public class HttpsOnlyMiddleware
    {
        private readonly RequestDelegate next;

        public HttpsOnlyMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.IsHttps)
            {
                context.Response.StatusCode = StatusCodes.Status505HttpVersionNotsupported;
                context.Response.Body = new MemoryStream(Encoding.UTF8.GetBytes("Use HTTPS insted of HTTP"));
                return;
            }

            await next.Invoke(context);
        }
    }
}
