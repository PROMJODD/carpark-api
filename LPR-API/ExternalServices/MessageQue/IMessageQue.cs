
namespace Prom.LPR.Api.ExternalServices.MessageQue
{
    public interface IMessageQue<T>
    {
        public void PublishMessage(T data, string topic);
    }
}
