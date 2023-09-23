
namespace Prom.LPR.Api.ExternalServices.Recognition
{
    public interface IImageAnalyzer
    {
        public string AnalyzeFile(string imagePath);
        public T? AnalyzeFile<T>(string imagePath);
    }
}
