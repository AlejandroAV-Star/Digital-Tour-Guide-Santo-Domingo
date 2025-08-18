using DTourGuide.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DTourGuide.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _db;

        public UploadController(IWebHostEnvironment env, AppDbContext db)
        {
            _env = env; _db = db;
        }

        [HttpPost("{placeId:int}"), Authorize(AuthenticationSchemes = Auth.BasicAuthHandler.SchemeName, Roles = "Admin")]
        [RequestSizeLimit(20_000_000)]
        public async Task<ActionResult> Upload(int placeId, IFormFile file)
        {
            var place = await _db.Places.FirstOrDefaultAsync(p => p.Id == placeId);
            if (place == null) return NotFound();

            var folder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(folder, fileName);
            using (var stream = System.IO.File.Create(fullPath))
                await file.CopyToAsync(stream);

            var url = $"/uploads/{fileName}";
            place.Photos.Add(new Domain.Entities.Photo { Url = url });
            await _db.SaveChangesAsync();

            return Ok(new { url });
        }
    }
}
