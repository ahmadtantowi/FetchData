using System;

namespace FetchData
{
    public interface IApiService<T> where T : class
    {
        T Initiated { get; }
        T Background { get; }
        T Speculative { get; }
    }
}