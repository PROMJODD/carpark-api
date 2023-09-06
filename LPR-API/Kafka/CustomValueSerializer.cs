using System.Text;
using System.Text.Json;
using Confluent.Kafka;

namespace Prom.LPR.Api.Kafka
{
    public class CustomValueSerializer<T> : ISerializer<T>
    {
        public byte[] Serialize(T data, SerializationContext context)
        {
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, typeof(T)));
        }
    }
}
