using Data.Data;
using Data.Data.Common;
using Data.Dto.CRUD.PlayingField;
using Data.Models;
using Data.Models.Interfaces;
using Data.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers.api
{
    [Route("api/playing-fields")]
    [ApiController]
    public class PlayingFieldApiController : ControllerBase
    {
        private readonly StadiumStore _store;
        private readonly IWebHostEnvironment _env;

        public PlayingFieldApiController(StadiumStore store, IWebHostEnvironment env)
        {
            _store = store;
            _env = env;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayingFieldDto>>> Get([FromQuery] string? search)
        {
            var playingFieldsQuery = _store.QueryPlayingFieldsAsync();
             
            if (!string.IsNullOrEmpty(search))
            {
                playingFieldsQuery = playingFieldsQuery.Where(pf => pf.Name.Contains(search));
            }

            var playingFields = await playingFieldsQuery
                .Select(PlayingFieldDto.ToDto())
                .ToListAsync();

            return Ok(playingFields);
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PlayingFieldDto>> GetById(Guid id)
        {
            var entity = await _store.FindByIdAsync(id);
            if (!entity.IsSuccess || entity.Value == null)
            {
                return NotFound();
            }

            return Ok(PlayingFieldDto.ToDto().Compile()(entity.Value));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<PlayingFieldDto>> Post([FromBody] StadiumFormDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var playingField = new PlayingField
            {
                Id = Guid.NewGuid()
            };

            playingField.Name = model.Name;
            playingField.Description = model.Description;
            playingField.Longitude = model.Longitude ?? 0;
            playingField.Latitude = model.Latitude ?? 0;
            playingField.ContactNumber = model.ContactNumber;
            playingField.Status = (FieldStatus)model.Status;
            playingField.IsOutdoor = model.IsOutdoor;
            playingField.SurfaceType = (SurfaceType)model.SurfaceType;

            var result = await _store.CreatePlayingField(playingField);
            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            if (model.ImageIds.Count > 0)
                await _store.LinkImagesToFieldAsync(result.Value, model.ImageIds);

            var createdDto = PlayingFieldDto.ToDto().Compile()(playingField);

            return CreatedAtAction(nameof(GetById), new { id = playingField.Id }, createdDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<PlayingFieldDto>> Put(Guid id, [FromBody] StadiumFormDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var foundResult = await _store.FindByIdAsync(id);
            if (!foundResult.IsSuccess || foundResult.Value == null)
                return NotFound();

            var playingField = foundResult.Value;


            playingField.Name = model.Name;
            playingField.Description = model.Description;
            playingField.Longitude = model.Longitude ?? 0;
            playingField.Latitude = model.Latitude ?? 0;
            playingField.ContactNumber = model.ContactNumber;
            playingField.Status = (FieldStatus)model.Status;
            playingField.IsOutdoor = model.IsOutdoor;
            playingField.SurfaceType = (SurfaceType)model.SurfaceType;

            var updateResult = await _store.UpdatePlayingField(playingField);
            if (!updateResult.IsSuccess)
                return BadRequest(updateResult.Errors);

            if (model.ImageIds.Count > 0)
                await _store.LinkImagesToFieldAsync(id, model.ImageIds);

            return NoContent(); // or Ok(updatedDto)
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _store.DeleteByIdAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound();
            }

            return NoContent();
        }


        [HttpGet("{id:guid}/images")]
        public async Task<IActionResult> GetImages(Guid id)
        {
            var result = await _store.FindByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound();

            var images = result.Value.Images.Select(img => new
            {
                id = img.Id,
                path = img.Path,
                fileName = img.FileName,
                sizeBytes = img.SizeBytes,
            });

            return Ok(images);
        }

        [Authorize(Roles = AppRoles.ADMIN_ROLE)]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] Guid? stadiumId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
            if (!allowedTypes.Contains(file.ContentType))
                return BadRequest("Unsupported file type.");

            var ext = Path.GetExtension(file.FileName);
            var uniqueName = $"{Guid.NewGuid()}{ext}";
            var folderPath = Path.Combine(_env.WebRootPath, "images", "stadiums");
            Directory.CreateDirectory(folderPath);
            var absolutePath = Path.Combine(folderPath, uniqueName);

            using (var stream = new FileStream(absolutePath, FileMode.Create))
                await file.CopyToAsync(stream);

            var image = new ImageResource
            {
                Id = Guid.NewGuid(),
                Path = $"/images/stadiums/{uniqueName}",
                FileName = file.FileName,
                SizeBytes = file.Length,
                ContentType = file.ContentType,
                UploadDate = DateTime.UtcNow,
                PlayingFieldId = stadiumId
            };

            await _store.AddImageResourceAsync(image);

            return Ok(new { id = image.Id, path = image.Path });
        }

        [Authorize(Roles = AppRoles.ADMIN_ROLE)]
        [HttpDelete("image/{id:guid}")]
        public async Task<IActionResult> DeleteImage(Guid id)
        {
            var imageResult = await _store.GetImageByIdAsync(id);
            if (!imageResult.IsSuccess)
                return NotFound();

            var image = imageResult.Value;
            var absolutePath = Path.Combine(_env.WebRootPath, image.Path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(absolutePath))
                System.IO.File.Delete(absolutePath);

            await _store.RemoveImageAsync(id);
            return Ok();
        }
    }
}
