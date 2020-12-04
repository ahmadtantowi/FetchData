using System;
using System.Net.Http;
using System.Text.Json;
using FetchData.HttpTools;
using FetchData.Serialization;
using Fusillade;
using Refit;

namespace FetchData
{
    public class ApiService<T> : IApiService<T> where T : class
    {
        private readonly RefitSettings _refitSettings;
        private readonly TimeSpan _timeout;
        private readonly string _baseEndpoint;

        public ApiService(int timeout = 60)
        {
            _timeout = TimeSpan.FromSeconds(timeout);
        }

        public ApiService(string baseEndpoint, int timeout = 60) : this(timeout)
        {
            _baseEndpoint = baseEndpoint;
            CheckBaseEndpoint(_baseEndpoint);
        }

        public ApiService(string baseEndpoint, SerializeNamingProperty serializeName, int timeout = 60) : this(baseEndpoint, timeout)
        {
            _refitSettings = new RefitSettings(new SystemTextJsonContentSerializer(new JsonSerializerOptions
            {
                PropertyNamingPolicy = serializeName switch
                {
                    SerializeNamingProperty.CamelCase => JsonNamingPolicy.CamelCase,
                    SerializeNamingProperty.SnakeCase => JsonSnakeCaseNamingPolicy.Instance,
                    _ => null
                }
            }));
        }

        public T Initiated(string baseEndpoint = null)
        {
            if (baseEndpoint != null)
                CheckBaseEndpoint(baseEndpoint);
                
            return CreateClient(new RateLimitedHttpMessageHandler(new HttpLoggingHandler(), Priority.UserInitiated), baseEndpoint);
        }

        public T Background(string baseEndpoint = null)
        {
            if (baseEndpoint != null)
                CheckBaseEndpoint(baseEndpoint);
                
            return CreateClient(new RateLimitedHttpMessageHandler(new HttpLoggingHandler(), Priority.Background), baseEndpoint);
        }

        public T Speculative(string baseEndpoint = null)
        {
            if (baseEndpoint != null)
                CheckBaseEndpoint(baseEndpoint);
            
            return CreateClient(new RateLimitedHttpMessageHandler(new HttpLoggingHandler(), Priority.Speculative), baseEndpoint);
        }

        private T CreateClient(HttpMessageHandler messageHandler, string baseEndpoint)
        {
            var client = new HttpClient(messageHandler)
            {
                BaseAddress = new Uri(baseEndpoint ?? _baseEndpoint ?? throw new InvalidOperationException("Base endpoint not provided")),
                Timeout = _timeout
            };

            return _refitSettings is null
                ? RestService.For<T>(client)
                : RestService.For<T>(client, _refitSettings);
        }

        private void CheckBaseEndpoint(string baseEndpoint)
        {
            if (!Uri.IsWellFormedUriString(baseEndpoint, UriKind.Absolute))
                throw new ArgumentException("Base endpoint is not correct");
        }
    }
}