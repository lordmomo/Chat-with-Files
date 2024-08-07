namespace GenAIApp.Models
{
    public class Document
    {
        public string Id { get; set; }
        public string FileName { get; set; }

        public string Text { get; set; }

        public DateTime UploadDateTime { get; set; }
    }
}
