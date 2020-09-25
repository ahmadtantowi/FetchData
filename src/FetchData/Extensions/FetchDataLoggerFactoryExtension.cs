using System;
using Microsoft.Extensions.Logging;

namespace FetchData.Extensions
{
    public static class FetchDataServiceProviderExtension
    {
        public static ILoggerFactory SetFetchDataLoggerFactory(this ILoggerFactory loggerFactory)
        {
            LogProvider.SetLogFactory(loggerFactory);

            return loggerFactory;
        }
    }
}