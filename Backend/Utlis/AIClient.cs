using GenAIApp.Models;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

using HuggingfaceHub;
using HuggingFace;
using Tokenizers.DotNet;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using Newtonsoft.Json.Linq;
using System.IO.Packaging;
using System.Threading.RateLimiting;
using System.Net;

namespace GenAIApp.Utlis
{
    public class AIClient
    {
        private readonly HttpClient _httpClient;

        public AIClient(
            HttpClient httpClient) {
            _httpClient = httpClient;
        }
        public async Task<ResponseMessage> GetAnswer(string question, string documentText)
        {

            var payload = new
            {
                inputs = new
                {
                    question = question,
                    context = documentText
                }
            };
           
            try
            {

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.apiKey);

                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(ApiUrls.questionAnsweringModelAPIUrl, content);
                response.EnsureSuccessStatusCode();

                Console.WriteLine(response.ToString());
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody.ToString());

                dynamic result = JsonConvert.DeserializeObject(responseBody);
                Console.WriteLine(result.ToString());

                var jsonResponse = JsonConvert.DeserializeObject<JObject>(responseBody);
                var answer = jsonResponse["answer"].ToString();
                var score = jsonResponse["score"]?.ToObject<double>() ?? 0.0;

                const double confidenceThreshold = 0.5;

                if(string.IsNullOrWhiteSpace(answer) || score < confidenceThreshold)
                {
                    return new ResponseMessage
                    {
                        StatusCode = HttpStatusCode.NoContent,
                        Success = false,
                        Message = "The question is irrelevant with the context provided"
                    };
                }
                Console.WriteLine("Answer");
                Console.WriteLine(answer);
                return new ResponseMessage { StatusCode = HttpStatusCode.OK, Success = true, Message = answer };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
                return new ResponseMessage { StatusCode = HttpStatusCode.InternalServerError, Success = false, Message = Message.internalServerErrorMessage + $"{ex}" };

            }
           
        }


        // Note :  status code TooManyRequests: {"error":"Rate limit reached.
        // You reached free usage limit (reset hourly). Please subscribe to a plan at https://huggingface.co/pricing to use the API at this rate"}
        // The free Inference API may be rate limited for heavy use cases.
        // We try to balance the loads evenly between all our available resources, and favoring steady flows of requests.
        // If your account suddenly sends 10k requests then you’re likely to receive 503 errors saying models are loading.
        // In order to prevent that, you should instead try to start running queries smoothly from 0 to 10k over the course of a few minutes.
        public async Task<ResponseMessage> GetSummary(string apiUrl, string textBlock)
        {

            //string passage = "New York (CNN)When Liana Barrientos was 23 years old, she got married in Westchester County, New York." +
            //    "A year later, she got married again in Westchester County, but to a different man and without divorcing her first husband." +
            //    "Only 18 days after that marriage, she got hitched yet again. Then, Barrientos declared ,I do, five more times, sometimes only within two weeks of each other." +
            //    "In 2010, she married once more, this time in the Bronx. In an application for a marriage license, she stated it was her,first and only marriage." +
            //    "Barrientos, now 39, is facing two criminal counts of,offering a false instrument for filing in the first degree, referring to her false statements on the 2010 marriage license application, " +
            //    "according to court documents.";

            var textLength = textBlock.Length;
            Console.WriteLine(textLength);
            var chunks = SplitIntoChunks(textBlock, 2000);
            Console.WriteLine(chunks.Count());
            //string passage = textBlock;
            var summaries = new List<string>();
            //var rateLimiter = new RateLimiter(TimeSpan.FromSeconds(1));

            int count = 1;

            foreach (var chunk in chunks)
            {
                var payload = new
                {
                    inputs = chunk
                };

                try
                {
                    //await rateLimiter.WaitAsync();

                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.apiKey);

                        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                        var response = await httpClient.PostAsync(apiUrl, content);
                        //response.EnsureSuccessStatusCode();

                        Console.WriteLine($"Request hit: {count}");
                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"Re Request hit: {count}");

                            var responseBody = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"Request failed with status code {response.StatusCode}: {responseBody}");

                            if (response.StatusCode == HttpStatusCode.InternalServerError)
                            {
                                Console.WriteLine("Server error. Retrying...");
                                //await Task.Delay(500); 
                                return await GetSummary(apiUrl, textBlock); 
                            }

                            return new ResponseMessage { StatusCode = HttpStatusCode.InternalServerError, Success = false, Message = Message.internalServerErrorMessage + $"{response.StatusCode}" };


                        }

                        Console.WriteLine($"Success Request hit: {count}");

                        count++;
                        var responseBodySuccess = await response.Content.ReadAsStringAsync();
                        var jsonArray = JArray.Parse(responseBodySuccess);
                        if (jsonArray.Count > 0)
                        {
                            var summaryText = jsonArray[0]["summary_text"].ToString();
                            summaries.Add(summaryText);
                        }
                        else
                        {
                            throw new InvalidOperationException("The response does not contain expected data.");
                        }

                    }
                } catch (Exception ex)
                {

                    Console.WriteLine($"Request error: {ex.Message}");
                    return new ResponseMessage { StatusCode = HttpStatusCode.InternalServerError, Success = false, Message = Message.internalServerErrorMessage + $"{ex}" };

                }
            }

            return new ResponseMessage { StatusCode = HttpStatusCode.OK, Success = true, Message = string.Join(" ",summaries) };


        }

        private List<string> SplitIntoChunks(string text, int chunkSize)
        {
            var chunks = new List<string>();
            for (int i = 0; i< text.Length; i+= chunkSize)
            {
                var chunk = text.Substring(i,Math.Min(chunkSize,text.Length - i));
                chunks.Add(chunk);
            }
            return chunks;
        }
    }
}
