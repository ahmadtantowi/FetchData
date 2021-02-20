using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace FetchData.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, params ApiServiceConfiguration[] configs)
        {
            foreach (var api in configs)
            {
                var allServices = Assembly
                    .GetExecutingAssembly().GetTypes()
                    .Where(a => api.Modules.Any(m => m.IsAssignableFrom(a)) && a.IsInterface);
                
                foreach (var module in api.Modules)
                {
                    var moduleServices = allServices.Where(m => module.IsAssignableFrom(m) && m != module);

                    foreach (var service in moduleServices)
                    {
                        var iApiService = typeof(IApiService<>).MakeGenericType(service);
                        var apiService = typeof(ApiService<>).MakeGenericType(service);

                        services.Add(new ServiceDescriptor(
                            iApiService, 
                            provider => Activator.CreateInstance(apiService, api, provider, api.DelegatingHandler), 
                            ServiceLifetime.Transient));
                    }
                }
            }

            return services;
        }
    }
}