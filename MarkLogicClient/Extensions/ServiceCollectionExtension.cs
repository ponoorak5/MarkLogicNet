using MarkLogicClient.Models.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System;

namespace MarkLogicClient.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddMarkLogicClient(this IServiceCollection services, MarkLogicClientConfiguration? config = null)
        {
            if (config != null)
            {
                services.AddSingleton<MarkLogicClientConfiguration>(o => new MarkLogicClientConfiguration()
                {
                    Login = config.Login,
                    Password = config.Password,
                    Url = config.Url,
                    Timeout = config.Timeout
                });
            }
            else
            {
                services.AddSingleton<MarkLogicClientConfiguration>();
            }

            services.AddTransient<MarkLogicClient>();
            services.AddHttpClient<MarkLogicClient>();
            return services;
        }
    }
}