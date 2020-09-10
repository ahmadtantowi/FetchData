using System;
using System.Net.Http;
using FetchData.HttpTools;
using Fusillade;
using Refit;

namespace FetchData
{
    public class ApiService<T> : IApiService<T> where T : class
    {
        private readonly TimeSpan _timeout;
        private readonly string _baseEndpoint;

        public ApiService(int timeout = 60)
        {
            _timeout = TimeSpan.FromSeconds(timeout);
        }

        public ApiService(string baseEndpoint, int timeout = 60) : this(timeout)
        {
            CheckBaseEndpoint(baseEndpoint);
            _baseEndpoint = baseEndpoint;
        }

        public T Initiated(string baseEndpoint = null)
        {
            CheckBaseEndpoint(baseEndpoint);
            return CreateClient(new RateLimitedHttpMessageHandler(new HttpLoggingHandler(), Priority.UserInitiated), baseEndpoint);
        }

        public T Background(string baseEndpoint = null)
        {
            CheckBaseEndpoint(baseEndpoint);
            return CreateClient(new RateLimitedHttpMessageHandler(new HttpLoggingHandler(), Priority.Background), baseEndpoint);
        }

        public T Speculative(string baseEndpoint = null)
        {
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

            return RestService.For<T>(client);
        }

        private void CheckBaseEndpoint(string baseEndpoint)
        {
            if (!Uri.IsWellFormedUriString(baseEndpoint, UriKind.Absolute))
                throw new ArgumentException("Base endpoint is not correct");
        }
    }
}