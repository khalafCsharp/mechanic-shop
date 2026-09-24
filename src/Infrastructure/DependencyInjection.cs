using System.Text;
using Microsoft.EntityFrameworkCore;
using Application.Common.Interfaces;
using Infrastructure.BackgroundJobs;
using Infrastructure.Data;
using Infrastructure.Identity;
using Infrastructure.Identity.AuthorizationPolicies;
using Infrastructure.RealTime;
using Infrastructure.Services;
using MechanicShop.Infrastructure.Data;
using MechanicShop.Infrastructure.Data.Interceptors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSignalR();
            services.AddSingleton(TimeProvider.System);

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            ArgumentNullException.ThrowIfNull(connectionString);

            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

            services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                     options.AddInterceptors(serviceProvider.GetServices<ISaveChangesInterceptor>());

                options.UseSqlServer(connectionString);

            });


            services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

            services.AddScoped<ApplicationDbContextInitialiser>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var jwtSettings = configuration.GetSection("JwtSettings");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                           Encoding.UTF8.GetBytes(jwtSettings["Secret"]!)),
                };
            });

            services
            .AddIdentityCore<AppUser>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 1;
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();

            services.AddScoped<IAuthorizationHandler, LaborAssignedRequirementHandler>();

            services.AddAuthorizationBuilder()
                  .AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager"))
                  .AddPolicy("SelfScopedWorkOrderAccess", policy =>
                    policy.Requirements.Add(new LaborAssignedRequirement()));

            services.AddTransient<IIdentityService, IdentityService>();

            services.AddHybridCache(options => options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(10), // L2, L3
                LocalCacheExpiration = TimeSpan.FromSeconds(30), // L1
            });
            services.AddScoped<IWorkOrderPolicy, WorkOrderPolicy>();

            services.AddScoped<ITokenProvider, TokenProvider>();

            services.AddScoped<IInvoicePdfGenerator, InvoicePdfGenerator>();

            services.AddScoped<INotificationService, NotificationService>();

            services.AddScoped<IWorkOrderNotifier, SignalRWorkOrderNotifier>();

            services.AddHostedService<StaleScheduledWorkOrdersCleaningService>();

            return services;
        }
    }
}