using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using FetchData.HttpTools;
using FetchData.Serialization;
using Fusillade;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace FetchData
{
    public class ApiService<T> : IApiService<T> 
        where T : class
    {
        public ApiConfiguration ApiConfig { get; set; }

        public T Initiated => _initiated.Value;
        public T Background => _background.Value;
        public T Speculative => _speculative.Value;
        
        private readonly Lazy<T> _initiated;
        private readonly Lazy<T> _background;
        private readonly Lazy<T> _speculative;
        private readonly RefitSettings _refitSettings;

        public ApiService(ApiConfiguration apiConfig)
            : this(apiConfig, null, null) {}

        public ApiService(ApiConfiguration apiConfig, IServiceProvider provider)
            : this(apiConfig, provider, null) {}

        public ApiService(ApiConfiguration apiConfig, IServiceProvider provider, Type handler)
        {
            var providedHandler = provider is null
                // provide handler without using IServiceProvider
                ? handler is null
                    ? new HttpLoggingHandler()
                    : Activator.CreateInstance(handler) as DelegatingHandler
                // get handler from IServiceProvider
                : handler is null 
                    ? provider.GetService<HttpLoggingHandler>()
                    : provider.GetService(handler) as DelegatingHandler;
            
            _initiated = new Lazy<T>(() => CreateClient(new RateLimitedHttpMessageHandler(providedHandler, Priority.UserInitiated)));
            _background = new Lazy<T>(() => CreateClient(new RateLimitedHttpMessageHandler(providedHandler, Priority.Background)));
            _speculative = new Lazy<T>(() => CreateClient(new RateLimitedHttpMessageHandler(providedHandler, Priority.Speculative)));

            _refitSettings = new RefitSettings(new SystemTextJsonContentSerializer(new JsonSerializerOptions
            {
                PropertyNamingPolicy = apiConfig.SerializeMode switch
                {
                    SerializeNamingProperty.CamelCase => JsonNamingPolicy.CamelCase,
                    SerializeNamingProperty.SnakeCase => JsonSnakeCaseNamingPolicy.Instance,
                    _ => null
                }
            }));

            ApiConfig = apiConfig;
        }

        private T CreateClient(HttpMessageHandler messageHandler)
        {
            var client = new HttpClient(messageHandler)
            {
                BaseAddress = new Uri(ApiConfig.Host ?? throw new InvalidOperationException("Host services not provided")),
                Timeout = TimeSpan.FromSeconds(ApiConfig.Timeout)
            };

            if (client.DefaultRequestHeaders.AcceptLanguage.Count == 0)
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(Thread.CurrentThread.CurrentCulture.Name));

            return _refitSettings is null
                ? RestService.For<T>(client)
                : RestService.For<T>(client, _refitSettings);
        }
    }
}