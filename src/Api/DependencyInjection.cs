namespace Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json.Serialization;
    using System.Threading.RateLimiting;
    using System.Threading.Tasks;
    using Api.Infrastructure;
    using Api.OpenApi.Transformers;
    using Application.Common.Interfaces;
    using Asp.Versioning;
    using global::Infrastructure.Settings;
    using Microsoft.AspNetCore.RateLimiting;
    using Serilog;
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
               QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            services.AddOutputCache()
            .AddCustomProblemDetails()
            .AddApiDocumentation()
            .AddExceptionHandling()
            .AddControllerWithJsonConfiguration()
            .AddIdentityInfrastructure()
            .AddAppRateLimiting()
            .AddCustomApiVersioning()
            .AddConfiguredCors(configuration);

            return services;
        }

        private static IServiceCollection AddCustomApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            }).AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }


        private static IServiceCollection AddAppRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.AddSlidingWindowLimiter("SlidingWindow", limiterOptions =>
                {
                    limiterOptions.PermitLimit = 100;
                    limiterOptions.Window = TimeSpan.FromMinutes(1);
                    limiterOptions.SegmentsPerWindow = 6;
                    limiterOptions.QueueLimit = 10;
                    limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    limiterOptions.AutoReplenishment = true;
                });

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            return services;
        }

        private static IServiceCollection AddConfiguredCors(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>()!;

            services.AddCors(options => options.AddPolicy(
                appSettings.CorsPolicyName,
                policy => policy
                    .WithOrigins(appSettings.AllowedOrigins!)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()));

            return services;
        }

        private static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IUser, CurrentUser>();
            services.AddHttpContextAccessor();
            return services;
        }

        private static IServiceCollection AddControllerWithJsonConfiguration(this IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options => options
                .JsonSerializerOptions
                .DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

            return services;
        }

        private static IServiceCollection AddExceptionHandling(this IServiceCollection services)
        {
            services.AddExceptionHandler<GlobalExceptionHandler>();
            return services;
        }

        private static IServiceCollection AddApiDocumentation(this IServiceCollection services)
        {
            string[] versions = ["v1"];

            foreach (var version in versions)
            {
                services.AddOpenApi(version, options =>
                {
                    // Versioning config
                    options.AddDocumentTransformer<VersionInfoTransformer>();

                    // Security Scheme config
                    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
                    options.AddOperationTransformer<BearerSecuritySchemeTransformer>();
                });
            }

            return services;
        }
        private static IServiceCollection AddCustomProblemDetails(this IServiceCollection services)
        {
            services.AddProblemDetails(options => options.CustomizeProblemDetails = (context) =>
            {
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions.Add("requestId", context.HttpContext.TraceIdentifier);
            });

            return services;
        }
        
        private static IServiceCollection AddOutputCache(this IServiceCollection services)
        {
            services.AddOutputCache(options =>
            {
                options.SizeLimit = 100 * 1024 * 1024;
                options.AddBasePolicy(policy => policy.Expire(TimeSpan.FromSeconds(60)));
            });

            return services;
        }
        public static IApplicationBuilder UseCoreMiddlewares(this IApplicationBuilder app, IConfiguration configuration)
        {

            app.UseExceptionHandler();


            app.UseStatusCodePages();


            app.UseHttpsRedirection();


            app.UseSerilogRequestLogging();


            app.UseCors(configuration["AppSettings:CorsPolicyName"]!);


            app.UseRateLimiter();


            app.UseAuthentication();


            app.UseAuthorization();


            app.UseOutputCache();

            return app;
        }
    }
}