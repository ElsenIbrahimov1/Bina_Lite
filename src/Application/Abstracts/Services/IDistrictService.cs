using Application.DTOs.District;

namespace Application.Abstracts.Services;

public interface IDistrictService
{
    Task CreateDistrictAsync(CreateDistrictRequest request, CancellationToken ct = default);
    Task<List<GetAllDistrictResponse>> GetAllDistrictsAsync(CancellationToken ct = default);
    Task<GetByIdDistrictResponse> GetDistrictByIdAsync(int id, CancellationToken ct = default);
    Task UpdateDistrictAsync(UpdateDistrictRequest request, CancellationToken ct = default);
    Task DeleteDistrictAsync(int id, CancellationToken ct = default);

    Task<List<GetAllDistrictResponse>> GetDistrictsByCityIdAsync(int cityId, CancellationToken ct = default);
}