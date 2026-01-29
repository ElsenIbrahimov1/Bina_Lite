using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.PropertyAd;
using Application.Shared.Helpers.Responses;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PropertyAdController : ControllerBase
{
    private readonly IPropertyAdService _service;

    public PropertyAdController(IPropertyAdService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
        => Ok(await _service.GetAllPropertyAdsAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
        => Ok(await _service.GetPropertyAdByIdAsync(id, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePropertyAdRequest request, CancellationToken ct)
    {
        await _service.CreatePropertyAdAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePropertyAdRequest request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("ID mismatch.");

        await _service.UpdatePropertyAdAsync(request, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _service.DeletePropertyAdAsync(id, ct);
        return NoContent();
    }

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(PropertyCategory category, CancellationToken ct)
        => Ok(await _service.GetPropertyAdsByCategoryAsync(category, ct));
}