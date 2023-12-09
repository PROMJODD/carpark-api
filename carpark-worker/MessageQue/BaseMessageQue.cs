using Prom.LPR.Worker.Models;

namespace Prom.LPR.Worker.MessageQue
{
    public abstract class BaseMessageQue : IMessageQue
    {
        protected abstract void Initlize();

        public void Init()
        {
            Initlize();
        }

        public virtual MJob? GetMessage()
        {
            return null;
        }

        public virtual void PutMessage(MJob job)
        {
        }        
    }
}
