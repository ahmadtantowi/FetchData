using System;
using Microsoft.Extensions.Logging;

namespace FetchData.Extensions
{
    public static class LoggerFactoryExtension
    {
        public static ILoggerFactory SetFetchDataLoggerFactory(this ILoggerFactory loggerFactory)
        {
            LogProvider.SetLogFactory(loggerFactory);

            return loggerFactory;
        }
    }
}