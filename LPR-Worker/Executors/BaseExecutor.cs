using Microsoft.Extensions.Configuration;
using Prom.LPR.Worker.Models;

namespace Prom.LPR.Worker.Executors
{
    public abstract class BaseExecutor : IExecutor
    {
        protected MJob jobParam = new MJob() { Id = ""};

        protected abstract void ThreadExecutor();
        protected abstract void Init();

        public Thread Execute(MJob job, IConfiguration cfg)
        {
            jobParam = job;

            Init();

            Thread t = new Thread(new ThreadStart(ThreadExecutor));
            t.Start();

            return t;
        }
    }
}
