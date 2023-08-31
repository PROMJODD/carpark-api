using Serilog;
using Prom.LPR.Worker.Models;
using Confluent.Kafka;

namespace Prom.LPR.Worker.MessageQue
{
    public class KafkaMQ : BaseMessageQue
    {
        private Queue<MJob> queue = new Queue<MJob>();
        private ConsumerConfig? consumerCfg = null;
        private readonly string topic = "";
        private readonly string groupId = "";
        private readonly string host = "";
        private readonly int port = 3333;

        public KafkaMQ(string topic, string group, string host, int port)
        {
            this.topic = topic;
            this.host = host;
            this.port = port;
            this.groupId = group;

            consumerCfg = new ConsumerConfig 
            {
                BootstrapServers = this.host,
                AutoOffsetReset = AutoOffsetReset.Latest,
                ClientId = "",
                GroupId = this.groupId,
                BrokerAddressFamily = BrokerAddressFamily.V4,
            };
        }

        private void PullMessage()
        {
            var consumer = new ConsumerBuilder<Ignore, string>(consumerCfg).Build();
            consumer.Subscribe(this.topic);

            try 
            {
                while (true) 
                {
                    var consumeResult = consumer.Consume();
                    MJob job = new MJob() { Message = consumeResult.Message.Value };
                    queue.Enqueue(job);
                }
            }
            catch (OperationCanceledException) 
            {
                // The consumer was stopped via cancellation token.
            } 
            finally 
            {
                consumer.Close();
            }
        }

        private void SubscribeKafka()
        {
            Thread t = new Thread(new ThreadStart(PullMessage));
            t.Start();
        }

        protected override void Initlize()
        {
            Log.Information($"Waiting for message(s) in Kafka topic=[{this.topic}], group=[{this.groupId}]...");
            SubscribeKafka();
        }

        public override MJob? GetMessage()
        {
            MJob? m = null;
            try
            {
                m = queue.Dequeue();
            }
            catch
            {
            }

            return m;
        }
    }
}
