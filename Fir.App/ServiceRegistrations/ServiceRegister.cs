using Fir.App.Context;
using Fir.App.Services.Implementations;
using Fir.App.Services.Interfaces;
using Fir.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fir.App.ServiceRegistrations
{
    public static class ServiceRegister
    {
        public static void Register(this IServiceCollection service,IConfiguration configuration)
        {
            service.AddScoped<IMailService, MailService>();
            service.AddScoped<IBasketService, BasketService>();
            service.AddIdentity<AppUser, IdentityRole>()
                   .AddDefaultTokenProviders()
                   .AddEntityFrameworkStores<FirDbContext>();
            service.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            });
            service.AddDbContext<FirDbContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("Default"));
            });

        }
    }
}
