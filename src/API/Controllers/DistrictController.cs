using Application.Abstracts.Services;
using Application.DTOs.District;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DistrictController : ControllerBase
{
    private readonly IDistrictService _service;

    public DistrictController(IDistrictService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
        => Ok(await _service.GetAllDistrictsAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
        => Ok(await _service.GetDistrictByIdAsync(id, ct));

    [HttpGet("by-city/{cityId:int}")]
    public async Task<IActionResult> GetByCity(int cityId, CancellationToken ct)
        => Ok(await _service.GetDistrictsByCityIdAsync(cityId, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDistrictRequest request, CancellationToken ct)
    {
        await _service.CreateDistrictAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDistrictRequest request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("ID mismatch.");

        await _service.UpdateDistrictAsync(request, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteDistrictAsync(id, ct);
        return NoContent();
    }
}