using Application.DTOs.PropertyAd;
using Application.Shared.Helpers.Responses;
using Domain.Enums;

namespace Application.Abstracts.Services;

public interface IPropertyAdService
{
    Task CreatePropertyAdAsync(CreatePropertyAdRequest request, string userId, CancellationToken ct = default);

    Task<List<GetALLPropertyAdResponse>> GetAllPropertyAdsAsync(CancellationToken ct = default);

    Task<GetByIdPropertyAdResponse> GetPropertyAdByIdAsync(int id, CancellationToken ct = default);

    Task UpdatePropertyAdAsync(UpdatePropertyAdRequest request, CancellationToken ct = default);

    Task DeletePropertyAdAsync(int id, CancellationToken ct = default);

    Task<List<GetALLPropertyAdResponse>> GetPropertyAdsByCategoryAsync(PropertyCategory category, CancellationToken ct = default);
}