using Insurance.Api.Configs;
using Insurance.Api.Consts;
using Insurance.Api.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading;

namespace Insurance.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            //in future when we will host out application we can configure cors policies
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }

        public static void AddHttpClientService(this IServiceCollection services)
        {
            // Configure HttpClient for the InsuranceService
            services.AddHttpClient($"{ApiConstants.ProductApiClient}", (serviceProvider, client) =>
            {
                var settings = serviceProvider
                    .GetRequiredService<IOptions<AppConfig>>().Value;
                client.BaseAddress = new Uri(settings.ProductApiUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(15)
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);

            // Configure HttpClient for the InsuranceService
            services.AddHttpClient($"{ApiConstants.SurchargeApiClient}", (serviceProvider, client) =>
            {
                var baseAddressProvider = serviceProvider.GetRequiredService<IBaseAddressProvider>();
                client.BaseAddress = new Uri(baseAddressProvider.GetBaseAddress());
            })
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(15),
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);


        }
    }
}

