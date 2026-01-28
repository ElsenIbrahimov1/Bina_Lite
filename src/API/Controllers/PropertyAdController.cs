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
    {
        BaseResponse<List<GetALLPropertyAdResponse>> response = await _service.GetAllPropertyAdsAsync(ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        BaseResponse<GetByIdPropertyAdResponse> response = await _service.GetPropertyAdByIdAsync(id, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePropertyAdRequest request, CancellationToken ct)
    {
        BaseResponse response = await _service.CreatePropertyAdAsync(request, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePropertyAdRequest request, CancellationToken ct)
    {
        if (id != request.Id)
            return BadRequest(BaseResponse.Fail("ID mismatch.", 400));

        BaseResponse response = await _service.UpdatePropertyAdAsync(request, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        BaseResponse response = await _service.DeletePropertyAdAsync(id, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(PropertyCategory category, CancellationToken ct)
    {
        BaseResponse<List<GetALLPropertyAdResponse>> response = await _service.GetPropertyAdsByCategoryAsync(category, ct);
        return StatusCode(response.StatusCode, response);
    }
}