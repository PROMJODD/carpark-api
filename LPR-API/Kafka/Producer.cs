using System.Diagnostics.CodeAnalysis;
using Confluent.Kafka;

namespace Prom.LPR.Api.Kafka
{
    [ExcludeFromCodeCoverage]
    public class Producer<T>
    {
        readonly string? kafkaHost;
        readonly string kafkaPort;

        public Producer(string host, string port)
        {
            kafkaHost = host;
            kafkaPort = port;
        }

        public ProducerConfig GetProducerConfig()
        {
            return new ProducerConfig
            {
                BootstrapServers = $"{kafkaHost}:{kafkaPort}"
            };
        }

        public void Produce(T data, string topic)
        {
            using (var producer = new ProducerBuilder<Null, T>(GetProducerConfig())
                .SetValueSerializer(new CustomValueSerializer<T>())
                .Build())
            {
                producer.Produce(topic, new Message<Null, T> { Value = data });
                producer.Flush();
            }
        }
    }
}
