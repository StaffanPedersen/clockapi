using Microsoft.AspNetCore.Mvc;

namespace ClockApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        private readonly string _imagePath = "/var/www/clockapi/wwwroot/images/blog";

        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm]string description, [FromForm]DateTime clientDate, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var filePath = Path.Combine(_imagePath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { fileName = file.FileName, description, clientDate });
        }
    }
}