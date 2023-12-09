
namespace Prom.LPR.Worker.Processors
{
    public abstract class BaseProcessor : IProcessor
    {
        protected abstract void ThreadProcess();
        protected abstract void Init();

        public void Run()
        {
            Init();

            Thread t = new Thread(new ThreadStart(ThreadProcess));
            t.Start();
        }
    }
}
