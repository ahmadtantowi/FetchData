using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using shortid;
using shortid.Configuration;

namespace FetchData.HttpTools
{
    public class HttpLoggingHandler : DelegatingHandler
    {
        private readonly string[] _types;
        private readonly ILogger _logger;

        public HttpLoggingHandler(HttpMessageHandler innerHandler = null) 
            : base(innerHandler ?? new HttpClientHandler())
        { 
            _types = new[] { "html", "text", "xml", "json", "txt" };
            _logger = LogProvider.GetLogger(nameof(HttpLoggingHandler));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string id = default, msg = default;
            DateTime start = default, end = default;

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                id = ShortId.Generate(new GenerationOptions
                {
                    Length = 8,
                    UseSpecialCharacters = false
                });
                msg = $"[{id} Request]";

                _logger.LogDebug("{0} ========Start==========", msg);
                _logger.LogDebug("{0} {1} {2} {3}/{4}", msg, request.Method, request.RequestUri.PathAndQuery, request.RequestUri.Scheme, request.Version);
                _logger.LogDebug("{0} Host: {1}://{2}", msg, request.RequestUri.Scheme, request.RequestUri.Host);

                foreach (var header in request.Headers)
                {
                    _logger.LogDebug("{0} {1}: {2}", msg, header.Key, string.Join(", ", header.Value));
                }

                if (request.Content != null)
                {
                    foreach (var header in request.Content.Headers)
                    {
                        _logger.LogDebug("{0} {1}: {2}", msg, header.Key, string.Join(", ", header.Value));
                    }

                    if (request.Content is StringContent || IsTextBasedContentType(request.Headers) || IsTextBasedContentType(request.Content.Headers))
                    {
                        var result = await request.Content.ReadAsStringAsync().ConfigureAwait(false);

                        _logger.LogDebug("{0} Content:", msg);
                        _logger.LogDebug("{0} {1}", msg, string.Join(string.Empty, result.Cast<char>()));
                    }
                }

                start = DateTime.Now;
            }

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                end = DateTime.Now;
                
                _logger.LogDebug("{0} Duration: {1}", msg, end - start);
                _logger.LogDebug("{0} ==========End==========", msg);

                msg = $"[{id} Response]";
                _logger.LogDebug("{0} =========Start=========", msg);

                var resp = response;

                _logger.LogDebug("{0} {1}/{2} {3} {4}", msg, request.RequestUri.Scheme.ToUpper(), resp.Version, (int)resp.StatusCode, resp.ReasonPhrase);

                foreach (var header in resp.Headers)
                {
                    _logger.LogDebug("{0} {1}: {2}", msg, header.Key, string.Join(", ", header.Value));
                }

                if (resp.Content != null)
                {
                    foreach (var header in resp.Content.Headers)
                    {
                        _logger.LogDebug("{0} {1}: {2}", msg, header.Key, string.Join(", ", header.Value));
                    }

                    if (resp.Content is StringContent || IsTextBasedContentType(resp.Headers) || IsTextBasedContentType(resp.Content.Headers))
                    {
                        start = DateTime.Now;
                        var result = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                        end = DateTime.Now;

                        _logger.LogDebug("{0} Content:", msg);
                        _logger.LogDebug("{0} {1}", msg, string.Join(string.Empty, result.Cast<char>()));
                        _logger.LogDebug("{0} Duration: {1}", msg, end - start);
                    }
                }
                _logger.LogDebug("{0} ==========End==========", msg);
            }

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