
namespace Prom.LPR.Api.ExternalServices.MessageQue
{
    public interface IMessageQue<in T>
    {
        public void PublishMessage(T data, string topic);
    }
}
