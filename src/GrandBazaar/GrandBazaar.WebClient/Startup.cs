using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GrandBazaar.WebClient.Data;
using GrandBazaar.WebClient.Models;
using GrandBazaar.WebClient.Services;
using GrandBazaar.Domain;
using System.IO;
using GrandBazaar.WebClient.Filters;

namespace GrandBazaar.WebClient
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            //Password Strength Setting
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            //Seting the Account Login page
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.LoginPath = "/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
                options.LogoutPath = "/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                options.AccessDeniedPath = "/Account/AccessDenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                options.SlidingExpiration = true;
            });


            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc(options =>
                options.Filters.Add<AccountAddressFilter>()
            );

            services.AddSingleton<IIpfsService, IpfsService>();
            services.AddSingleton<IEthereumService>(scope =>
            {
                string url = "https://ropsten.infura.io/R13BuegiIhZLUGVL3Qdq";
                string abi = File.ReadAllText("contractabi.json");
                string contractAddress = "0x549ee4703e6beec593253e612bbb3d8c318808af";
                return new EthereumService(url, abi, contractAddress);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
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
                    name: "areas",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            CreateUserRoles(services).Wait();
        }

        private async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            IdentityResult createRoleResult;
            string[] roles = new[] { "Admin", "Seller", "Customer" };
            foreach (string role in roles)
            {
                bool roleCheck = await roleManager.RoleExistsAsync(role);
                if (!roleCheck)
                {
                    createRoleResult = await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            string[] emails = new[]
            {
                "davolio@gmail.com",
                "andrew.fuller@yahoo.com",
                "jleverling@yahoo.co.uk",
                "stbuchanan@microsoft.com"
            };

            IdentityResult createUserResult;
            foreach (string email in emails)
            {
                ApplicationUser existingUser = await userManager.FindByNameAsync(email);
                if (existingUser == null)
                {
                    var user = new ApplicationUser { UserName = email, Email = email };
                    createUserResult = await userManager.CreateAsync(user, "123456");
                }
            }

            ApplicationUser nancy = await userManager.FindByNameAsync(emails[0]);
            ApplicationUser andrew = await userManager.FindByNameAsync(emails[1]);
            ApplicationUser janet = await userManager.FindByNameAsync(emails[2]);
            ApplicationUser steven = await userManager.FindByNameAsync(emails[3]);

            IdentityResult addToRoleResult = await userManager.AddToRoleAsync(nancy, "Admin");
            addToRoleResult = await userManager.AddToRolesAsync(andrew, new[] { "Admin", "Seller" });
            addToRoleResult = await userManager.AddToRoleAsync(janet, "Seller");
            addToRoleResult = await userManager.AddToRoleAsync(steven, "Customer");
        }
    }
}
