
using GenAIApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace GenAIApp.Services.ServiceInterface
{
    public interface QuestionServiceInterface
    {
        Task<ResponseMessage> askQuestion(IFormFile file, string query);
        Task<ResponseMessage> AskQuestionWithDocumentId(string documentId, string query);
        Task<ResponseMessage> GenerateSummary(IFormFile file);
        Task<ResponseMessage> UploadDocument(IFormFile file);

        Task<ResponseMessage> GenerateSummaryWithDocumentId(string documentId);
    }
}
