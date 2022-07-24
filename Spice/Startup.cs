using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Spice.Data;
using Spice.Models;
using Spice.Utility;
using Stripe;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Spice
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
            var env = Environment.GetEnvironmentVariable("DATABASE_URL");
            var raw = env.Split("/");

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = raw[2].Split("@")[1].Split(":")[0],
                Username = raw[2].Split(":")[0],
                Password = raw[2].Split(":")[1].Split("@")[0],
                Port = Convert.ToInt32(raw[2].Split(":")[2]),
                Database = raw[3]
            };

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.ToString()));

            //services.AddDatabaseDeveloperPageExceptionFilter();
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            //services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaims>();
            services.Configure<StripeSettings>(x => { x.PublishableKey = Environment.GetEnvironmentVariable("PUBLISHABLE_KEY"); x.SecretKey = Environment.GetEnvironmentVariable("SECRET_KEY"); });
            services.Configure<AppSettings>(x=>x.Domain = Environment.GetEnvironmentVariable("DATABASE_URL"));
            
            services.AddControllersWithViews();
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddSession(o =>
            {
                o.Cookie.IsEssential = true;
                o.IdleTimeout = TimeSpan.FromMinutes(30);
                o.Cookie.HttpOnly = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInit)
        {
            StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("SECRET_KEY");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            dbInit.Initialize().GetAwaiter();


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            CultureInfo customCulture = new CultureInfo("pl-PL");
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            CultureInfo.DefaultThreadCurrentCulture = customCulture;
            CultureInfo.DefaultThreadCurrentUICulture = customCulture;

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
