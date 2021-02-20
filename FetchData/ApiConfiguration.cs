using FetchData.Serialization;

namespace FetchData
{
    public class ApiConfiguration
    {
        public ApiConfiguration(string host, int timeout = 180, SerializeNamingProperty serializeMode = SerializeNamingProperty.Default)
        {
            Host = host;
            Timeout = timeout;
            SerializeMode = serializeMode;
        }

        public ApiConfiguration()
            : this(null) {}

        public string Host { get; set; }
        public int Timeout { get; set; }
        public SerializeNamingProperty SerializeMode { get; set; }
    }
}