using Microsoft.Extensions.Configuration;
using Prom.LPR.Worker.Models;

namespace Prom.LPR.Worker.Executors
{
    public interface IExecutor
    {
        public Thread Execute(MJob job, IConfiguration cfg);
    }
}
