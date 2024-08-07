using DocumentFormat.OpenXml.Office2016.Excel;
using GenAIApp.Models;
using GenAIApp.Services.ServiceInterface;
using GenAIApp.Utlis;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GenAIApp.Services.ServiceImplementation
{
    public class QuestionServiceImplementation : QuestionServiceInterface
    {

        private readonly DocumentProcessor _documentProcessor;
        private readonly AIClient _aiClient;
        private readonly DocumentRepository _documentRepository;

        public QuestionServiceImplementation(DocumentProcessor documentProcessor
            , AIClient aiClient
            , DocumentRepository documentRepository
            )
        {
            _documentProcessor = documentProcessor;
            _aiClient = aiClient;
            _documentRepository = documentRepository;
        }

        public async Task<ResponseMessage> UploadDocument(IFormFile file)
        {
            if (file == null)
            {
                return new ResponseMessage { StatusCode = HttpStatusCode.BadRequest, Success = false, Message = Message.fileCannotBeNullErrorMessage };
            }


            try
            {
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                string documentText;
                using (var stream = file.OpenReadStream())
                {
                    switch (fileExtension)
                    {
                        case ".pdf":
                            documentText = _documentProcessor.ExtractTextFromPdfStream(stream);
                            break;
                        case ".docx":
                            documentText = _documentProcessor.ExtractTextFromWordStream(stream);
                            break;
                        default:
                            return new ResponseMessage { StatusCode = HttpStatusCode.BadRequest, Success = false, Message = Message.unsupportedFileTypeErrorMessage};
                    }
                }

                if (documentText == null)
                {
                    return new ResponseMessage { StatusCode = HttpStatusCode.NotFound, Success = false, Message = Message.textCannotBeExtractedErrorMessage };
                }

                var documentId = Guid.NewGuid().ToString(); 
                var document = new Document { Id = documentId, FileName = file.FileName, Text = documentText };
                await _documentRepository.AddDocumentAsync(document);

                return new ResponseMessage { StatusCode = HttpStatusCode.OK, Success = true, Message = documentId };
            }
            catch (Exception ex)
            {
                return new ResponseMessage { StatusCode = HttpStatusCode.InternalServerError, Success = false, Message = Message.internalServerErrorMessage + $": {ex}"};
            }
        }

        public async Task<ResponseMessage> AskQuestionWithDocumentId(string documentId, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new ResponseMessage { StatusCode = HttpStatusCode.BadRequest, Success = false, Message = Message.questionRequiredErrorMessage };
            }

            try
            {
                var document = await _documentRepository.GetDocumentAsync(documentId);
                if (document == null)
                {
                    return new ResponseMessage { StatusCode = HttpStatusCode.NotFound, Success = false, Message = Message.documentNotFoundErrorMessage };
                }

                var answer = await _aiClient.GetAnswer(query, document.Text);
                return answer;
            }
            catch (Exception ex)
            {
                return new ResponseMessage { StatusCode = HttpStatusCode.InternalServerError, Success = false, Message = Message.internalServerErrorMessage + $"{ex}" };
            }
        }

        public async Task<ResponseMessage> GenerateSummary(IFormFile file)
        {
            if (file == null)
            {
                return new ResponseMessage { StatusCode = HttpStatusCode.BadRequest, Success = false, Message = Message.fileCannotBeNullErrorMessage };
            }

            try
            {
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                string documentText;
                switch (fileExtension)
                {
                    case ".pdf":
                        using(var stream = file.OpenReadStream())
                        {
                            documentText = _documentProcessor.ExtractTextFromPdfStream(stream);
                            break;
                        }
                    case ".docx":
                        using (var stream = file.OpenReadStream())
                        {
                            documentText = _documentProcessor.ExtractTextFromWordStream(stream);
                            break;
                        }

                    default:
                        return new ResponseMessage { StatusCode = HttpStatusCode.BadRequest, Success = false, Message = Message.unsupportedFileTypeErrorMessage};

                }
                if (documentText == null)
                {
                    return new ResponseMessage { StatusCode = HttpStatusCode.NotFound, Success = false, Message = Message.textCannotBeExtractedErrorMessage };
                }

                var answer = await _aiClient.GetSummary(ApiUrls.textSummarizationModelAPIUrl, documentText);
                return answer;

            }
            catch (Exception ex)
            {
                return new ResponseMessage { StatusCode = HttpStatusCode.InternalServerError, Success = false, Message = Message.internalServerErrorMessage + $"{ ex}" };
            }

        }

        public async Task<ResponseMessage> askQuestion(IFormFile file, string query)
        {
            if(file == null)
            {
                return new ResponseMessage { StatusCode = HttpStatusCode.BadRequest, Success = false, Message = Message.fileCannotBeNullErrorMessage };

            }
            if (string.IsNullOrWhiteSpace(query))
            {
                return new ResponseMessage { StatusCode = HttpStatusCode.BadRequest, Success = false, Message = Message.questionRequiredErrorMessage };

            }

            try
            {
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                string documentText;
                switch (fileExtension)
                {
                    case ".pdf":
                        using (var stream = file.OpenReadStream())
                        {

                            documentText = _documentProcessor.ExtractTextFromPdfStream(stream);
                            break;
                        }
                    case ".docx":
                        using (var stream = file.OpenReadStream())
                        {

                            documentText = _documentProcessor.ExtractTextFromWordStream(stream);
                            break;
                        }
                    default:
                        return new ResponseMessage{ StatusCode = HttpStatusCode.BadRequest, Success = false, Message = Message.unsupportedFileTypeErrorMessage };
                }
                if (documentText == null)
                {
                    return new ResponseMessage { StatusCode = HttpStatusCode.NotFound, Success = false, Message = Message.textCannotBeExtractedErrorMessage };
                }

                var answer = await _aiClient.GetAnswer(query, documentText);
                return answer;

            }
            catch (Exception ex)
            {
                return new ResponseMessage { StatusCode = HttpStatusCode.InternalServerError, Success = false, Message = Message.internalServerErrorMessage + $" {ex}" };

            }
        }

        public async Task<ResponseMessage> GenerateSummaryWithDocumentId(string documentId)
        {
            try
            {
                var document = await _documentRepository.GetDocumentAsync(documentId);
                if (document == null)
                {
                    return new ResponseMessage { StatusCode = HttpStatusCode.NotFound, Success = false, Message = Message.documentNotFoundErrorMessage };
                }

                Console.WriteLine(document.ToString());
                Console.WriteLine(document.Text);
                var answer = await _aiClient.GetSummary(ApiUrls.textSummarizationModelAPIUrl, document.Text);
                return answer;
            }
            catch (Exception ex)
            {
                return new ResponseMessage { StatusCode = HttpStatusCode.InternalServerError, Success = false, Message = Message.internalServerErrorMessage + $"{ex}" };
            }
        }
    }
}
