
using System.Diagnostics.CodeAnalysis;

namespace Prom.LPR.Api.ExternalServices.MessageQue
{
    [ExcludeFromCodeCoverage]
    public class MessageQueKafka<T> : IMessageQue<T>
    {
        private Producer<T> producer;

        public MessageQueKafka(string host, string port)
        {
            producer = new Producer<T>(host, port);
        }

        public void PublishMessage(T data, string topic)
        {
            producer.Produce(data, topic);
        }
    }
}
