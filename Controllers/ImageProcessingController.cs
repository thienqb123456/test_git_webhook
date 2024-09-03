using Microsoft.AspNetCore.Mvc;
using ngaoda.Services;

namespace ngaoda.Controllers
{
    public class ImageProcessingController(IImageProcessingService imageProcessingService) : Controller
    {

        [HttpGet("ImageProcessing")]
        public IActionResult ProcessImage(string path, int w = 680, int h = 0, int q = 100, int dpr = 1, string fit = "fill")
        {
            try
            {
                var response = imageProcessingService.ProcessImage(path, w, h, q, dpr, fit);
                return File(response.fileContent, response.contentType);
            }
            catch(Exception exception)
            {
                throw;
            }
            
        }
    }
}
