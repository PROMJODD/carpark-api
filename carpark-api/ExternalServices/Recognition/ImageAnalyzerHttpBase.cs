using Serilog;
using System.Text.Json;

namespace Prom.LPR.Api.ExternalServices.Recognition
{
    public abstract class ImageAnalyzerHttpBase : IImageAnalyzer
    {
        public abstract string GetFormUploadKey();
        public abstract HttpClient GetHttpClient();
        public abstract HttpRequestMessage GetRequestMessage();

        public T? AnalyzeFile<T>(string imagePath)
        {
            var json = AnalyzeFile(imagePath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            try
            {
                var obj = JsonSerializer.Deserialize<T>(json, options);
                return obj;
            }
            catch (Exception e)
            {
                Log.Error("Data seem to be not JSON : [{0}]", json);
                Log.Error(e.Message);
            }

            return default;
        }

        public string AnalyzeFile(string imagePath)
        {
            var client = GetHttpClient();
            var requestMessage = GetRequestMessage();

            var key = GetFormUploadKey();
            using var stream = File.OpenRead(imagePath);
            using var content = new MultipartFormDataContent
            {
                { new StreamContent(stream), key, imagePath }
            };

            requestMessage.Content = content;
            var task = client.SendAsync(requestMessage);

            var analyzeResult = "";
            try
            {
                var response = task.Result;
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().Result;

                analyzeResult = responseBody;
                //Use Console.WriteLine because Serilog has the issue when showing JSON curry brace
                Console.WriteLine(responseBody);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            return analyzeResult;
        }
    }
}
