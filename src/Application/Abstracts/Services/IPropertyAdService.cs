using Application.DTOs.PropertyAd;
using Application.Shared.Helpers.Responses;
using Domain.Enums;

namespace Application.Abstracts.Services;

public interface IPropertyAdService
{
    Task<BaseResponse<List<GetALLPropertyAdResponse>>> GetAllPropertyAdsAsync(CancellationToken ct = default);

    Task<BaseResponse<GetByIdPropertyAdResponse>> GetPropertyAdByIdAsync(int id, CancellationToken ct = default);

    Task<BaseResponse> CreatePropertyAdAsync(CreatePropertyAdRequest request, CancellationToken ct = default);

    Task<BaseResponse> UpdatePropertyAdAsync(UpdatePropertyAdRequest request, CancellationToken ct = default);

    Task<BaseResponse> DeletePropertyAdAsync(int id, CancellationToken ct = default);

    Task<BaseResponse<List<GetALLPropertyAdResponse>>> GetPropertyAdsByCategoryAsync(PropertyCategory category, CancellationToken ct = default);
}
