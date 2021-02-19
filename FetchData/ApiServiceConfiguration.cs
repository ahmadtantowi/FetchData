using System;
using System.Collections.Generic;

namespace FetchData
{
    public class ApiServiceConfiguration
    {
        /// <summary>
        /// Api service configuration include host, timeout & serialize mode
        /// </summary>
        public ApiConfiguration Configuration { get; set; }

        /// <summary>
        /// Http delegate handler, 
        /// provide this value with inherited <see cref="DelegatingHandler"/> if custom handler needed
        /// </summary>
        public Type DelegatingHandler { get; set; }

        /// <summary>
        /// Interface of base modules,
        /// the interface should not contain any method or property
        /// </summary>
        public IEnumerable<Type> Modules { get; set; }
    }
}