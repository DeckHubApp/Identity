using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Slidable.Identity.Areas.Identity.Services;
using Slidable.Identity.Data;

[assembly: HostingStartup(typeof(Slidable.Identity.Areas.Identity.IdentityHostingStartup))]
namespace Slidable.Identity.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddTransient<IEmailSender, EmailSender>();


                services.AddMvc()
                    .AddRazorPagesOptions(options =>
                    {
                        options.AllowAreas = true;
                        options.Conventions.AuthorizeFolder("/Account/Manage");
                        options.Conventions.AuthorizePage("/Account/Logout");
                    });

                services.ConfigureApplicationCookie(options => 
                {
                    options.LoginPath = "/Identity/Account/Login";
                    options.LogoutPath = "/Identity/Account/Logout";
                    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                });
            });
        }
    }
}