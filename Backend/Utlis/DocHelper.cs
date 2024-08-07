using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace GenAIApp.Utlis
{
    public class DocHelper
    {

        public void callServerlessApi(IFormFile file, string apiKey, string payload)
        {


        }

        public async Task<string> summaryGeneration(string apiUrl, string text) 
        {
            var payload = new
            {
                inputs = text
            };
            try
            { 
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.apiKey);

                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();

                Console.WriteLine(response.ToString());
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody.ToString());
                dynamic result = JsonConvert.DeserializeObject(responseBody);
                Console.WriteLine(result.ToString());

                var jsonResponse = JsonConvert.DeserializeObject<JObject>(responseBody);
                var answer = jsonResponse["answer"].ToString();


                return answer;                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
                return $"Error: {ex}";
            }
        }
    }
}
