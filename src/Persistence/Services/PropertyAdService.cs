using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.PropertyAd;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Services;

public sealed class PropertyAdService : IPropertyAdService
{
    private readonly IPropertyAdRepository _repository;

    public PropertyAdService(IPropertyAdRepository repository)
    {
        _repository = repository;
    }

    public async Task CreatePropertyAdAsync(CreatePropertyAdRequest request, CancellationToken ct = default)
    {
        var entity = new PropertyAd
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            IsMortgage = request.IsMortgage,
            IsExtract = request.IsExtract,
            Price = request.Price,
            RoomCount = request.RoomCount,
            AreaInSquareMeters = request.AreaInSquareMeters,
            OfferType = request.OfferType,
            PropertyCategory = request.PropertyCategory
        };

        await _repository.AddAsync(entity, ct);
        await _repository.SaveChangesAsync(ct);
    }

    public async Task<List<GetALLPropertyAdResponse>> GetAllPropertyAdsAsync(CancellationToken ct = default)
    {
        return await _repository.Query()
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .Select(x => new GetALLPropertyAdResponse
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description
            })
            .ToListAsync(ct);
    }

    public async Task<GetByIdPropertyAdResponse?> GetPropertyAdByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.Query()
            .AsNoTracking()
            .Include(x => x.Media)
            .Where(x => x.Id == id)
            .Select(x => new GetByIdPropertyAdResponse
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Media = x.Media,
                IsMortgage = x.IsMortgage,
                IsExtract = x.IsExtract,
                Price = x.Price,
                RoomCount = x.RoomCount,
                AreaInSquareMeters = x.AreaInSquareMeters,
                OfferType = x.OfferType,
                PropertyCategory = x.PropertyCategory
            })
            .FirstOrDefaultAsync(ct);
    }


    public async Task<bool> UpdatePropertyAdAsync(UpdatePropertyAdRequest request, CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (request.Id <= 0) return false;
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Title is required.", nameof(request.Title));

        var entity = await _repository.GetByIdAsync(request.Id, ct);
        if (entity is null) return false;

        entity.Title = request.Title.Trim();
        entity.Description = request.Description?.Trim() ?? string.Empty;
        entity.IsMortgage = request.IsMortgage;
        entity.IsExtract = request.IsExtract;
        entity.Price = request.Price;
        entity.RoomCount = request.RoomCount;
        entity.AreaInSquareMeters = request.AreaInSquareMeters;
        entity.OfferType = request.OfferType;
        entity.PropertyCategory = request.PropertyCategory;

        _repository.Update(entity);
        await _repository.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeletePropertyAdAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0) return false;

        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return false;

        _repository.Delete(entity);
        await _repository.SaveChangesAsync(ct);
        return true;
    }

    public async Task<List<GetALLPropertyAdResponse>> GetPropertyAdsByCategoryAsync(PropertyCategory category, CancellationToken ct = default)
    {
        return await _repository.Query()
            .AsNoTracking()
            .Where(x => x.PropertyCategory == category)
            .OrderByDescending(x => x.Id)
            .Select(x => new GetALLPropertyAdResponse
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description
            })
            .ToListAsync(ct);
    }

}
