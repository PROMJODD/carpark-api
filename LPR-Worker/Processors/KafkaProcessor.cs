using Serilog;
using System.Collections;
using Prom.LPR.Worker.MessageQue;
using Prom.LPR.Worker.Executors;
using Microsoft.Extensions.Configuration;
using Prom.LPR.Worker.Utils;
using Serilog.Sinks.Syslog;

namespace Prom.LPR.Worker.Processors
{
    public class KafkaProcessor : BaseProcessor
    {
        private readonly IConfiguration configuration;
        private Hashtable threadMap = new Hashtable();

        protected IMessageQue messageQue = new KafkaMQ("", "", 9999);

        protected override void Init()
        {
            var topic = ConfigUtils.GetConfig(configuration, "kafka:topic");
            var host = ConfigUtils.GetConfig(configuration, "kafka:host");
            var port = ConfigUtils.GetConfig(configuration, "kafka:port");

            Log.Information($"Started Kafka processor Topic=[{topic}], Host=[{host}], Port=[{port}]");

            messageQue = new KafkaMQ(topic, host, port.ToInt());
            messageQue.Init();
        }

        public KafkaProcessor(IConfiguration cfg)
        {
            configuration = cfg;
        }

        public void SetMessageQue(IMessageQue mq)
        {
            messageQue = mq;
        }

        private void CleanupThread()
        {
            ArrayList arr = new ArrayList();

            foreach (int key in threadMap.Keys)
            {
                Thread? t = (Thread?) threadMap[key];
                if (t != null)
                {
                    if (t.ThreadState.Equals(ThreadState.Stopped))
                    {                    
                        arr.Add(key);
                    }
                }
            }

            foreach (int key in arr)
            {
                threadMap.Remove(key);
            }
        }

        protected override void ThreadProcess()
        {
            while (true)
            {
                if (threadMap.Count >= 5)
                {
                    Log.Information("Thread count is above the limit, do nothing");
                }
                else
                {
                    var job = messageQue.GetMessage();
                    if (job != null)
                    {
                        //In the futrue we can read from job.Type
                        var executor = ExecutorFactory.GetExecutor("LPR", configuration);
                        var t = executor.Execute(job, configuration);

                        threadMap.Add(t.ManagedThreadId, t);
                    }
                }

                CleanupThread();
                Thread.Sleep(2000);
            }
        }
    }
}
