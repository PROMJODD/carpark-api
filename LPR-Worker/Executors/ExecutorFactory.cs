using Microsoft.Extensions.Configuration;

namespace Prom.LPR.Worker.Executors
{
    public class ExecutorFactory
    {
        public static IExecutor GetExecutor(string type, IConfiguration cfg)
        {
            if (type.Equals("LPR"))
            {
                return new LprExecutor(cfg);
            }

            //Can be other Executor in the future
            return new LprExecutor(cfg);
        }
    }
}
