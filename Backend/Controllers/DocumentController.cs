using GenAIApp.Utlis;
using Microsoft.AspNetCore.Mvc;

namespace GenAIApp.Controllers
{
    public class DocumentController : Controller
    {
        public DocumentController()
        {
            if (!Directory.Exists(Constants.documentStoragePath))
            {
                Directory.CreateDirectory(Constants.documentStoragePath);
            }
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost("upload-file")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var filePath = Path.Combine(Constants.documentStoragePath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }


            return Ok(new { FilePath = filePath });
        }
    }
}
