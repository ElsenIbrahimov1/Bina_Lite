using Application.DTOs.PropertyAd;
using Domain.Enums;

namespace Application.Abstracts.Services;

public interface IPropertyAdService
{
    Task<List<GetALLPropertyAdResponse>> GetAllPropertyAdsAsync(CancellationToken ct = default);

    Task<GetByIdPropertyAdResponse?> GetPropertyAdByIdAsync(int id, CancellationToken ct = default);

    Task  CreatePropertyAdAsync(CreatePropertyAdRequest request, CancellationToken ct = default);

    Task<bool> UpdatePropertyAdAsync(UpdatePropertyAdRequest request, CancellationToken ct = default);
    Task<bool> DeletePropertyAdAsync(int id, CancellationToken ct = default);

    Task<List<GetALLPropertyAdResponse>> GetPropertyAdsByCategoryAsync(PropertyCategory category, CancellationToken ct = default);
}
