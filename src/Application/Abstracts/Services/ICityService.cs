using Application.DTOs.City;

namespace Application.Abstracts.Services;

public interface ICityService
{
    Task CreateCityAsync(CreateCityRequest request, CancellationToken ct = default);
    Task<List<GetAllCityResponse>> GetAllCitiesAsync(CancellationToken ct = default);
    Task<GetByIdCityResponse> GetCityByIdAsync(int id, CancellationToken ct = default);
    Task UpdateCityAsync(UpdateCityRequest request, CancellationToken ct = default);
    Task DeleteCityAsync(int id, CancellationToken ct = default);
}