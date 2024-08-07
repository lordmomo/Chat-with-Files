using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;
using UglyToad.PdfPig;

namespace GenAIApp.Utlis
{
    public class DocumentProcessor
    {
        //public string ExtractTextFromPdf(string filePath)
        //{
        //    var sb = new StringBuilder();
        //    using (var document = PdfDocument.Open(filePath))
        //    {
        //        foreach (var page in document.GetPages())
        //        {
        //            sb.AppendLine(page.Text);
        //        }
        //    }
        //    return sb.ToString();
        //}
        //public string ExtractTextFromWord(string filePath)
        //{
        //    var sb = new StringBuilder();
        //    using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
        //    {

        //        var body = wordDoc.MainDocumentPart.Document.Body;
        //        foreach (var paragraph in body.Elements<Paragraph>())
        //        {
        //            sb.AppendLine(paragraph.InnerText);
        //        }
        //    }
        //    return sb.ToString();
        //}

        public string ExtractTextFromPdfStream(Stream stream)
        {
            var sb = new StringBuilder();
            using (var document = PdfDocument.Open(stream))
            {
                foreach (var page in document.GetPages())
                {
                    sb.AppendLine(page.Text);
                }
            }
            return sb.ToString();
        }

        public string ExtractTextFromWordStream(Stream stream)
        {
            var sb = new StringBuilder();
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, false))
            {
                var body = wordDoc.MainDocumentPart.Document.Body;
                foreach (var paragraph in body.Elements<Paragraph>())
                {
                    sb.AppendLine(paragraph.InnerText);
                }
            }
            return sb.ToString();
        }
    }
}
