using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.District;
using Application.Shared.Helpers.Exceptions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using FluentValidation.Results;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Services;

public sealed class DistrictService : IDistrictService
{
    private readonly IDistrictRepository _districtRepository;
    private readonly ICityRepository _cityRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateDistrictRequest> _createValidator;
    private readonly IValidator<UpdateDistrictRequest> _updateValidator;

    public DistrictService(
        IDistrictRepository districtRepository,
        ICityRepository cityRepository,
        IMapper mapper,
        IValidator<CreateDistrictRequest> createValidator,
        IValidator<UpdateDistrictRequest> updateValidator)
    {
        _districtRepository = districtRepository;
        _cityRepository = cityRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    private static List<string> ToErrorList(ValidationResult result)
        => result.Errors.Select(e => e.ErrorMessage).Distinct().ToList();

    public async Task CreateDistrictAsync(CreateDistrictRequest request, CancellationToken ct = default)
    {
        if (request is null)
            throw new BadRequestException("Request is null.");

        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            throw new BadRequestException("Validation failed.", ToErrorList(validation));

        // Ensure city exists
        var cityExists = await _cityRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Id == request.CityId, ct);

        if (!cityExists)
            throw new BadRequestException("City does not exist.");

        var trimmed = (request.Name ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
            throw new BadRequestException("Name is required.");

        // Format: First letter uppercase, rest lowercase
        var formattedName = char.ToUpperInvariant(trimmed[0]) + trimmed[1..].ToLowerInvariant();
        var normalizedName = formattedName.ToLowerInvariant();

        // Unique per city check (case-insensitive)
        var exists = await _districtRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.CityId == request.CityId && x.Name.ToLower() == normalizedName, ct);

        if (exists)
            throw new BadRequestException("District with this name already exists in this city.");

        request.Name = formattedName;

        var entity = _mapper.Map<District>(request);

        await _districtRepository.AddAsync(entity, ct);
        await _districtRepository.SaveChangesAsync(ct);
    }

    public async Task<List<GetAllDistrictResponse>> GetAllDistrictsAsync(CancellationToken ct = default)
    {
        return await _districtRepository.Query()
            .AsNoTracking()
            .OrderBy(x => x.CityId)
            .ThenBy(x => x.Name)
            .ProjectTo<GetAllDistrictResponse>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<GetByIdDistrictResponse> GetDistrictByIdAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0)
            throw new BadRequestException("Invalid id.");

        var entity = await _districtRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (entity is null)
            throw new NotFoundException("District not found.");

        return _mapper.Map<GetByIdDistrictResponse>(entity);
    }

    public async Task UpdateDistrictAsync(UpdateDistrictRequest request, CancellationToken ct = default)
    {
        if (request is null)
            throw new BadRequestException("Request is null.");

        var validation = await _updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            throw new BadRequestException("Validation failed.", ToErrorList(validation));

        var entity = await _districtRepository.GetByIdAsync(request.Id, ct);
        if (entity is null)
            throw new NotFoundException("District not found.");

        // Ensure city exists
        var cityExists = await _cityRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Id == request.CityId, ct);

        if (!cityExists)
            throw new BadRequestException("City does not exist.");

        var trimmed = (request.Name ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
            throw new BadRequestException("Name is required.");

        var formattedName = char.ToUpperInvariant(trimmed[0]) + trimmed[1..].ToLowerInvariant();
        var normalizedName = formattedName.ToLowerInvariant();

        var exists = await _districtRepository.Query()
            .AsNoTracking()
            .AnyAsync(x =>
                x.Id != request.Id &&
                x.CityId == request.CityId &&
                x.Name.ToLower() == normalizedName,
                ct);

        if (exists)
            throw new BadRequestException("District with this name already exists in this city.");

        request.Name = formattedName;

        _mapper.Map(request, entity);

        _districtRepository.Update(entity);
        await _districtRepository.SaveChangesAsync(ct);
    }

    public async Task DeleteDistrictAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0)
            throw new BadRequestException("Invalid id.");

        var entity = await _districtRepository.GetByIdAsync(id, ct);
        if (entity is null)
            throw new NotFoundException("District not found.");

        _districtRepository.Delete(entity);
        await _districtRepository.SaveChangesAsync(ct);
    }

    public async Task<List<GetAllDistrictResponse>> GetDistrictsByCityIdAsync(int cityId, CancellationToken ct = default)
    {
        if (cityId <= 0)
            throw new BadRequestException("Invalid cityId.");

        return await _districtRepository.Query()
            .AsNoTracking()
            .Where(x => x.CityId == cityId)
            .OrderBy(x => x.Name)
            .ProjectTo<GetAllDistrictResponse>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }
}