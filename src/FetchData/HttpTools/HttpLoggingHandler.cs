using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using shortid;
using shortid.Configuration;

namespace FetchData.HttpTools
{
    public class HttpLoggingHandler : DelegatingHandler
    {
        private readonly string[] _types;
        private readonly ILogger _logger;
        private readonly GenerationOptions _shortIdOptions;

        public HttpLoggingHandler(HttpMessageHandler innerHandler = null)
            : base(innerHandler ?? new HttpClientHandler())
        {
            _types = new[] { "html", "text", "xml", "json", "txt" };
            _logger = LogProvider.GetLogger(nameof(HttpLoggingHandler));
            _shortIdOptions = new GenerationOptions
            {
                Length = 8,
                UseNumbers = false,
                UseSpecialCharacters = false
            };
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var id = ShortId.Generate(_shortIdOptions);
            var msg = $"[{id} Req]";

            _logger.LogInformation($"{msg} ========Start==========");
            _logger.LogInformation($"{msg} {request.Method} {request.RequestUri.PathAndQuery} {request.RequestUri.Scheme}/{request.Version}");
            _logger.LogInformation($"{msg} Host: {request.RequestUri.Scheme}://{request.RequestUri.Host}");

            foreach (var header in request.Headers)
            {
                _logger.LogInformation($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
            }

            if (request.Content != null)
            {
                foreach (var header in request.Content.Headers)
                {
                    _logger.LogInformation($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
                }

                if (request.Content is StringContent || IsTextBasedContentType(request.Headers) || IsTextBasedContentType(request.Content.Headers))
                {
                    var result = await request.Content.ReadAsStringAsync().ConfigureAwait(false);

                    _logger.LogInformation($"{msg} Content:");
                    _logger.LogInformation($"{msg} {string.Join("", result.Cast<char>())}");

                }
            }

            var start = DateTime.Now;
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var end = DateTime.Now;

            _logger.LogInformation($"{msg} Duration: {end - start}");
            _logger.LogInformation($"{msg} ==========End==========");

            msg = $"[{id}-Res]";
            _logger.LogInformation($"{msg} =========Start=========");

            var resp = response;

            _logger.LogInformation($"{msg} {request.RequestUri.Scheme.ToUpper()}/{resp.Version} {(int)resp.StatusCode} {resp.ReasonPhrase}");

            foreach (var header in resp.Headers)
            {
                _logger.LogInformation($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
            }

            if (resp.Content != null)
            {
                foreach (var header in resp.Content.Headers)
                {
                    _logger.LogInformation($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
                }

                if (resp.Content is StringContent || IsTextBasedContentType(resp.Headers) || IsTextBasedContentType(resp.Content.Headers))
                {
                    start = DateTime.Now;
                    var result = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                    end = DateTime.Now;

                    _logger.LogInformation($"{msg} Content:");
                    _logger.LogInformation($"{msg} {string.Join("", result.Cast<char>())}");
                    _logger.LogInformation($"{msg} Duration: {end - start}");
                }
            }
            _logger.LogInformation($"{msg} ==========End==========");

            return response;
        }

        private bool IsTextBasedContentType(HttpHeaders headers)
        {
            if (headers is null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            if (!headers.TryGetValues("Content-Type", out var values))
            {
                return false;
            }

            var header = string.Join(" ", values).ToLowerInvariant();

            return _types.Any(header.Contains);
        }
    }
}