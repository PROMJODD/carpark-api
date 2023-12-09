using Prom.LPR.Worker.Models;

namespace Prom.LPR.Worker.MessageQue
{
    public interface IMessageQue
    {
        public void Init();
        public MJob? GetMessage();
    }
}
