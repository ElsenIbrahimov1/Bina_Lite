using Application.Abstracts.Services;
using Application.DTOs.City;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CityController : ControllerBase
{
    private readonly ICityService _service;

    public CityController(ICityService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
        => Ok(await _service.GetAllCitiesAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
        => Ok(await _service.GetCityByIdAsync(id, ct));

    [Authorize(Policy = Policies.ManageCities)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCityRequest request, CancellationToken ct)
    {
        await _service.CreateCityAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created);
    }


    [Authorize(Policy = Policies.ManageCities)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCityRequest request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("ID mismatch.");

        await _service.UpdateCityAsync(request, ct);
        return NoContent();
    }


    [Authorize(Policy = Policies.ManageCities)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteCityAsync(id, ct);
        return NoContent();
    }
}