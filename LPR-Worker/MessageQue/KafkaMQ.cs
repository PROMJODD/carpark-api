using Serilog;
using Prom.LPR.Worker.Models;

namespace Prom.LPR.Worker.MessageQue
{
    public class KafkaMQ : BaseMessageQue
    {
        private Queue<MJob> queue = new Queue<MJob>();
        private readonly string topic = "";
        private readonly string host = "";
        private readonly int port = 3333;

        public KafkaMQ(string topic, string host, int port)
        {
            this.topic = topic;
            this.host = host;
            this.port = port;
        }

        private void PullMessage()
        {
            while (true)
            {
                //Do something here
                //Read from Kafka
                Thread.Sleep(1000);
            }
        }

        private void SubscribeKafka()
        {
            Thread t = new Thread(new ThreadStart(PullMessage));
            t.Start();
        }

        protected override void Initlize()
        {
            Log.Information("Waiting for message(s) in Kafka...");
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
