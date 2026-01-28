using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.PropertyAd;
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
    public async Task<ActionResult<List<GetALLPropertyAdResponse>>> Get(CancellationToken ct)
    {
        var result = await _service.GetAllPropertyAdsAsync(ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetByIdPropertyAdResponse>> Get(int id, CancellationToken ct)
    {
        var result = await _service.GetPropertyAdByIdAsync(id, ct);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePropertyAdRequest request,
        CancellationToken ct)
    {
        await _service.CreatePropertyAdAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created);
    }


    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePropertyAdRequest request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("ID mismatch.");

        var ok = await _service.UpdatePropertyAdAsync(request, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _service.DeletePropertyAdAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(PropertyCategory category, CancellationToken ct)
    {
        var result = await _service.GetPropertyAdsByCategoryAsync(category, ct);
        return Ok(result);
    }
}