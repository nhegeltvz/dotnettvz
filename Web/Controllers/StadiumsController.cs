using Data.Data;
using Data.Data.Common;
using Data.Dto.CRUD.PlayingField;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Controllers;

[Route("stadiums")]
public class StadiumsController : Controller
{
    private readonly StadiumStore _store;
    private readonly IWebHostEnvironment _env;

    public StadiumsController(StadiumStore store, IWebHostEnvironment env)
    {
        _store = store;
        _env = env;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var fields = await _store.GetAllStadiumsAsync();
        return View("StadiumsView", fields);
    }

    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {


        var playingFieldResult = await _store.FindByIdAsync(id);

        if (playingFieldResult is null || !playingFieldResult.IsSuccess)
        {
            return NotFound();
        }

        var playingField = playingFieldResult.Value;

        var playedMatchesCount = playingField.MatchRecords.Count(match => match.WasMatchHeld);

        var model = new StadiumDetailsViewModel
        {
            Field = playingField,
            PlayedMatchesCount = playedMatchesCount
        };

        return View("StadiumDetailsView", model);
    }
    [HttpGet("data")]
    public async Task<IActionResult> GetAll(string? search)
    {
        var playingFields = await _store.GetAllStadiumsAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            playingFields = playingFields
                .Where(field => (field.Name ?? string.Empty)
                    .Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return Json(playingFields);
    }

    [HttpGet("form")]
    public IActionResult Form() => PartialView("_StadiumForm", new StadiumFormDto());

    [HttpGet("getById/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var playingField = await _store.FindByIdAsync(id);
        return Json(playingField.Value);
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

        return Json(images);
    }

    [HttpPost("create")]
    [Consumes("application/json")]
    public async Task<IActionResult> Create([FromBody] StadiumFormDto playingFieldForm)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var playingField = new PlayingField
        {
            Id = Guid.NewGuid()
        };
        
        playingField.Name = playingFieldForm.Name;
        playingField.Description = playingFieldForm.Description;
        playingField.Longitude = playingFieldForm.Longitude ?? 0;
        playingField.Latitude= playingFieldForm.Latitude ?? 0;
        playingField.ContactNumber= playingFieldForm.ContactNumber;
        playingField.Status= (FieldStatus)playingFieldForm.Status;
        playingField.IsOutdoor= playingFieldForm.IsOutdoor;
        playingField.SurfaceType= (SurfaceType)playingFieldForm.SurfaceType;

        var result = await _store.CreatePlayingField(playingField);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        if (playingFieldForm.ImageIds.Count > 0)
            await _store.LinkImagesToFieldAsync(result.Value, playingFieldForm.ImageIds);

        return Ok();
    }

    [HttpPost("edit/{id:guid}")]
    [Consumes("application/json")]
    public async Task<IActionResult> Edit([FromBody] StadiumFormDto playingFieldForm, Guid id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var playingFieldResult = await _store.FindByIdAsync(id);

        if (playingFieldResult is null || !playingFieldResult.IsSuccess)
            return NotFound();

        var playingField = playingFieldResult.Value;

        playingField.Name = playingFieldForm.Name;
        playingField.Description = playingFieldForm.Description;
        playingField.Longitude = playingFieldForm.Longitude ?? 0;
        playingField.Latitude = playingFieldForm.Latitude ?? 0;
        playingField.ContactNumber = playingFieldForm.ContactNumber;
        playingField.Status = (FieldStatus)playingFieldForm.Status;
        playingField.IsOutdoor = playingFieldForm.IsOutdoor;
        playingField.SurfaceType = (SurfaceType)playingFieldForm.SurfaceType;

        var result = await _store.UpdatePlayingField(playingField);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        if (playingFieldForm.ImageIds.Count > 0)
            await _store.LinkImagesToFieldAsync(id, playingFieldForm.ImageIds);

        return Ok();
    }

    [HttpDelete("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _store.DeleteByIdAsync(id);
        return Ok();
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file, [FromQuery] Guid? stadiumId)
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
