using GenAIApp.Models;

namespace GenAIApp.Utlis
{
    public class DocumentRepository
    {
        private readonly Dictionary<string, Document> _documents = new Dictionary<string, Document>();

        public Task AddDocumentAsync(Document document)
        {
            _documents[document.Id] = document;
            return Task.CompletedTask;
        }

        public Task<Document> GetDocumentAsync(string id)
        {
            _documents.TryGetValue(id, out var document);
            return Task.FromResult(document);
        }
    }
}
