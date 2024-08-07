using DocumentFormat.OpenXml.Office2010.Word;
using GenAIApp.Models;
using GenAIApp.Services.ServiceInterface;
using GenAIApp.Utlis;
using HuggingFace;
using HuggingfaceHub;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using Microsoft.ML.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Tokenizers.DotNet;

namespace GenAIApp.Controllers
{

    public class QuestionController : Controller
    {

        //const string apiKey = "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";
        

        private readonly QuestionServiceInterface _questionServiceInterface;

        public QuestionController(
            QuestionServiceInterface questionServiceInterface
            )
        {
            
            _questionServiceInterface = questionServiceInterface;
            
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument(IFormFile file)
        {
            var response = await _questionServiceInterface.UploadDocument(file);
            return Json(response);
        }

        [HttpPost("ask-with-documentId")]
        public async Task<IActionResult> AskQuestionWithDocumentId([FromBody] QueryRequest request)
        {
            var response = await _questionServiceInterface.AskQuestionWithDocumentId(request.DocumentId, request.Query);
            return Json(response);
        }

        [HttpPost("ask")]
        public async Task<IActionResult> AskQuestion(IFormFile file, string query)
        {
           

            var response = await _questionServiceInterface.askQuestion(file,query);
            return Json(response);
          
        }

        [HttpPost("summary")]
        public async Task<IActionResult> Summary(IFormFile file)
        {
            var response = await _questionServiceInterface.GenerateSummary(file);

            return Json(response);
            
        }


        [HttpPost("summary-documentId")]
        public async Task<IActionResult> SummaryWithDOcumentId([FromBody] QueryRequest request)
        {
            var response = await _questionServiceInterface.GenerateSummaryWithDocumentId(request.DocumentId);
            return Json(response);

        }

        //[HttpPost("questionOnly")]
        //public async Task<IActionResult> QuestionOnly(string question)
        //{
        //    if (question == null)
        //    {
        //        return BadRequest("Request cannot be null.");
        //    }



        //    try
        //    {
        //        using var client = new HttpClient();
        //        var api = new HuggingFaceApi(apiKey, client);
        //        var response = await api.GenerateTextAsync(
        //            RecommendedModelIds.Gpt2,
        //            new GenerateTextRequest
        //            {
        //                Inputs = question,
        //                Parameters = new GenerateTextRequestParameters
        //                {
        //                    Max_new_tokens = 250,
        //                    Return_full_text = false,
        //                },
        //                Options = new GenerateTextRequestOptions
        //                {
        //                    Use_cache = true,
        //                    Wait_for_model = false,
        //                }
        //            }
        //            );
        //        Console.WriteLine(response);

        //        return Ok(new { Answer = response });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex}");
        //    }
        //}



        //[HttpPost("questionTokenizer")]
        //public async Task<IActionResult> QuestionTokenizer(string question)
        //{
        //    var hubName = "deepset/roberta-base-squad2";

        //    //var model = await HFDownloader.DownloadSnapshotAsync(hubName);


        //    var filePath = "tokenizer_config.json";
        //    var fileFullPath = await HuggingfaceHub.HFDownloader.DownloadFileAsync(hubName, filePath);
        //    Console.WriteLine($"Downloaded {fileFullPath}");

        //    var tokenizer = new Tokenizer(vocabPath: fileFullPath);
        //    var tokens = tokenizer.Encode(question);
        //    Console.WriteLine($"Encoded: {string.Join(", ", tokens)}");
        //    var decoded = tokenizer.Decode(tokens);
        //    Console.WriteLine($"Decoded: {decoded}");

        //    return Ok(decoded);
        //}

        //[HttpPost("answerQuestion")]
        //public async Task<IActionResult >AnswerQuestion(QueryPayload inputData)
        //{
        //    //var model = _mlContext.Model.Load("deepset-roberta-base-squad2", out _);
        //    //var predictor = _mlContext.Model.CreatePredictionEngine<Question, Answer>(model);

        //    //var question = new Question
        //    //{
        //    //    QuestionText = inputData.Inputs.Question,
        //    //    ContextText = inputData.Inputs.Context
        //    //};

        //    //var answer = predictor.Predict(question);

        //    //return Ok(answer);
        //    try
        //    {
        //        var answer = await GetAnswerFromHuggingFaceAsync(inputData.Inputs.Question, inputData.Inputs.Context);
        //        return Ok(answer);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }

        //}

        ////await DownloadModelAsync("deepset/roberta-base-squad2", "path/to/local/model/deepset-roberta-base-squad2");

        //public async Task DownloadModelAsync(string modelId, string destinationPath)
        //{
        //    var client = new HttpClient();
        //    var modelUrl = $"https://huggingface.co/{modelId}/resolve/main/pytorch_model.bin"; 
        //    var response = await client.GetAsync(modelUrl);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write);
        //        await response.Content.CopyToAsync(fileStream);
        //        fileStream.Close();
        //    }
        //    else
        //    {
        //        throw new Exception($"Failed to download model. Status code: {response.StatusCode}");
        //    }
        //}


        //public async Task<JObject> GetAnswerFromHuggingFaceAsync(string question, string context)
        //{
        //    var apiKey = "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"; 
        //    var url = "https://api-inference.huggingface.co/models/deepset/roberta-base-squad2";

        //    var requestBody = new
        //    {
        //        inputs = new
        //        {
        //            question = question,
        //            context = context
        //        }
        //    };

        //    using var client = new HttpClient();
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        //    var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        //    var response = await client.PostAsync(url, content);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var responseContent = await response.Content.ReadAsStringAsync();
        //        return JObject.Parse(responseContent);
        //    }
        //    else
        //    {
        //        throw new Exception($"Failed to get answer. Status code: {response.StatusCode}");
        //    }
        //}


      
    }
}
