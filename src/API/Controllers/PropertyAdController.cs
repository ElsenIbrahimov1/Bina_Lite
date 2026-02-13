using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.PropertyAd;
using Application.DTOs.PropertyAdMedia;
using Application.Shared.Helpers.Responses;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PropertyAdController : ControllerBase
{
    private readonly IPropertyAdService _service;
    private readonly IFileStorageService _storage;
    private readonly IPropertyMediaRepository _mediaRepo;
    private readonly IPropertyAdRepository _adRepo;

    public PropertyAdController(
        IPropertyAdService service,
        IFileStorageService storage,
        IPropertyMediaRepository mediaRepo,
        IPropertyAdRepository adRepo)
    {
        _service = service;
        _storage = storage;
        _mediaRepo = mediaRepo;
        _adRepo = adRepo;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
        => Ok(await _service.GetAllPropertyAdsAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
        => Ok(await _service.GetPropertyAdByIdAsync(id, ct));


    [Authorize(Policy = Policies.ManageProperties)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePropertyAdRequest request, CancellationToken ct)
    {
        await _service.CreatePropertyAdAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created);
    }


    [Authorize(Policy = Policies.ManageProperties)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePropertyAdRequest request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("ID mismatch.");

        await _service.UpdatePropertyAdAsync(request, ct);
        return NoContent();
    }


    [Authorize(Policy = Policies.ManageProperties)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _service.DeletePropertyAdAsync(id, ct);
        return NoContent();
    }

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(PropertyCategory category, CancellationToken ct)
        => Ok(await _service.GetPropertyAdsByCategoryAsync(category, ct));

    // --------- MEDIA (MinIO) ---------

    [Authorize(Policy = Policies.ManageProperties)]
    [HttpPost("{propertyAdId:int}/media")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadMedia(
        int propertyAdId,
        [FromForm] PropertyMediaUploadRequest request,
        CancellationToken ct)
    {
        if (propertyAdId <= 0) return BadRequest("Invalid propertyAdId.");
        if (request.File is null || request.File.Length == 0) return BadRequest("File is required.");
        if (request.Order < 0) return BadRequest("Order must be 0 or greater.");

        var adExists = await _adRepo.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Id == propertyAdId, ct);

        if (!adExists) return NotFound("PropertyAd not found.");

        // MinIO SaveAsync expects a seekable stream -> copy to MemoryStream
        await using var ms = new MemoryStream();
        await request.File.CopyToAsync(ms, ct);
        ms.Position = 0;

        var objectKey = await _storage.SaveAsync(
            content: ms,
            fileName: request.File.FileName,
            contentType: request.File.ContentType,
            propertyAdId: propertyAdId,
            ct: ct);

        var media = new PropertyMedia
        {
            PropertyAdId = propertyAdId,
            ObjectKey = objectKey,
            Order = request.Order
        };

        await _mediaRepo.AddAsync(media, ct);
        await _mediaRepo.SaveChangesAsync(ct);

        return StatusCode(StatusCodes.Status201Created, new
        {
            media.Id,
            media.ObjectKey,
            media.Order
        });
    }


    [Authorize(Policy = Policies.ManageProperties)]
    [HttpDelete("media/{mediaId:int}")]
    public async Task<IActionResult> DeleteMedia(int mediaId, CancellationToken ct)
    {
        if (mediaId <= 0) return BadRequest("Invalid mediaId.");

        var media = await _mediaRepo.GetByIdAsync(mediaId, ct);
        if (media is null) return NotFound("Media not found.");

        await _storage.DeleteFileAsync(media.ObjectKey, ct);

        _mediaRepo.Delete(media);
        await _mediaRepo.SaveChangesAsync(ct);

        return NoContent();
    }
}