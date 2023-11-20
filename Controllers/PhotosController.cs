using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using images.api.Data;
using images.api.Models;

namespace images.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly imagesapiContext _context;

        public PhotosController(imagesapiContext context)
        {
            _context = context;
        }

        // GET: api/Photos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Photo>>> GetPhoto()
        {
          if (_context.Photo == null)
          {
              return NotFound();
          }
            return await _context.Photo.ToListAsync();
        }

        // GET: api/Photos/md/5
        [HttpGet("{size}/{id}")]
        public async Task<ActionResult> GetPhoto(string size, int id)
        {  
            var photo = await _context.Photo.FindAsync(id);

            if (photo == null)
            {
                return NotFound();
            }

            byte[] bytes = System.IO.File.ReadAllBytes("C://images/" + size + "/" + photo.Filename);

            return File(bytes, photo.MimeType);
        }

        // POST: api/Photos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Photo>> PostPhoto()
        {
            try
            {
                IFormCollection form = await Request.ReadFormAsync();
                IFormFile file = form.Files.First();

                Image image = Image.Load(file.OpenReadStream());
                Photo photo = new Photo();

                photo.MimeType = file.ContentType;
                photo.Filename = Guid.NewGuid().ToString();
                if(photo.MimeType == "image/jpeg")
                {
                    photo.Filename += ".jpg";
                } 
                else
                {
                    photo.Filename += ".jfif";
                }

                SaveImage(image, photo.Filename);

                _context.Photo.Add(photo);
                _context.SaveChanges();

                return Ok(photo);

            }
            catch(Exception e)
            {
                return BadRequest(e);
            }
        }

        private void SaveImage(Image image, string filename)
        {
            image.Save("C://images/lg/" + filename);
            image.Mutate(i =>
            {
                i.Resize(new ResizeOptions()
                {
                    Mode = ResizeMode.Min,
                    Size = new Size() { Height = 720 }
                });
            });
            image.Save("C://images/md/" + filename);
            image.Mutate(i =>
            {
                i.Resize(new ResizeOptions()
                {
                    Mode = ResizeMode.Min,
                    Size = new Size() { Height = 320 }
                });
            });
            image.Save("C://images/sm/" + filename);
        }

        // DELETE: api/Photos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            if (_context.Photo == null)
            {
                return NotFound();
            }
            var photo = await _context.Photo.FindAsync(id);
            if (photo == null)
            {
                return NotFound();
            }

            _context.Photo.Remove(photo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PhotoExists(int id)
        {
            return (_context.Photo?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
