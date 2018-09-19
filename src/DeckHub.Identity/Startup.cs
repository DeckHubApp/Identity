using System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeckHub.Identity.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RendleLabs.DeckHub.Cookies;
using StackExchange.Redis;

namespace DeckHub.Identity
{
    [PublicAPI]
    public class Startup
    {
        private static ConnectionMultiplexer _connectionMultiplexer;
        private readonly IHostingEnvironment _env;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _env = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IApiKeyProvider, ApiKeyProvider>();
            services.AddDbContextPool<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("Identity")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Stores.MaxLengthForKeys = 128;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o => { o.Cookie.Path = "/"; })
                .AddTwitter(o =>
                {
                    o.ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"];
                    o.ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"];
                })
                .AddMicrosoftAccount(o =>
                {
                    o.ClientId = Configuration["Authentication:Microsoft:ApplicationId"];
                    o.ClientSecret = Configuration["Authentication:Microsoft:Password"];
                });

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, DeckHubClaimsPrincipalFactory>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var redisHost = Configuration.GetSection("Redis").GetValue<string>("Host");
            if (!string.IsNullOrWhiteSpace(redisHost))
            {
                var redisPort = Configuration.GetSection("Redis").GetValue<int>("Port");
                if (redisPort == 0)
                {
                    redisPort = 6379;
                }

                _connectionMultiplexer = ConnectionMultiplexer.Connect($"{redisHost}:{redisPort}");
                services.AddSingleton(_connectionMultiplexer);
            }

            if (!_env.IsDevelopment())
            {
                var dpBuilder = services.AddDataProtection().SetApplicationName("deckhub");
                if (_connectionMultiplexer != null)
                {
                    dpBuilder.PersistKeysToRedis(_connectionMultiplexer, "DataProtection:Keys");
                }
            }
            else
            {
                services.AddDataProtection()
                    .DisableAutomaticKeyGeneration()
                    .SetApplicationName("deckhub");
            }

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddMetrics();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment() || string.Equals(Configuration["Runtime:DeveloperExceptionPage"], "true",
                    StringComparison.OrdinalIgnoreCase))
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.Use(async (context, next) =>
                {
                    // Override scheme to be https
                    context.Request.Scheme = "https";
                    await next();
                });

                app.UseExceptionHandler("/Home/Error");
            }

            var pathBase = Configuration["Runtime:PathBase"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

//            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseDeckHubCookieWriter();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}