using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.City;
using Application.Shared.Helpers.Exceptions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using FluentValidation.Results;


namespace Persistence.Services;

public sealed class CityService : ICityService
{
    private readonly ICityRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateCityRequest> _createValidator;
    private readonly IValidator<UpdateCityRequest> _updateValidator;

    public CityService(
        ICityRepository repository,
        IMapper mapper,
        IValidator<CreateCityRequest> createValidator,
        IValidator<UpdateCityRequest> updateValidator)
    {
        _repository = repository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    private static List<string> ToErrorList(ValidationResult result)
    {
        return result.Errors.Select(e => e.ErrorMessage).Distinct().ToList();
    }

    public async Task CreateCityAsync(CreateCityRequest request, CancellationToken ct = default)
    {
        if (request is null)
            throw new BadRequestException("Request is null.");

        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            throw new BadRequestException("Validation failed.", ToErrorList(validation));

        var trimmed = (request.Name ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
            throw new BadRequestException("Name is required.");

        // store formatted (First letter uppercase, rest lowercase)
        var formattedName = char.ToUpperInvariant(trimmed[0]) + trimmed[1..].ToLowerInvariant();

        // normalized only for comparison
        var normalizedName = formattedName.ToLowerInvariant();

        var exists = await _repository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Name.ToLower() == normalizedName, ct);

        if (exists)
            throw new BadRequestException("City with this name already exists.");

        request.Name = formattedName;

        var entity = _mapper.Map<City>(request);

        await _repository.AddAsync(entity, ct);
        await _repository.SaveChangesAsync(ct);
    }




    public async Task<List<GetAllCityResponse>> GetAllCitiesAsync(CancellationToken ct = default)
    {
        var cities = await _repository.Query()
            .AsNoTracking()
            .Include(x => x.Districts)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

        return cities.Select(x => _mapper.Map<GetAllCityResponse>(x)).ToList();
    }


    public async Task<GetByIdCityResponse> GetCityByIdAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0)
            throw new BadRequestException("Invalid id.");

        var city = await _repository.Query()
            .AsNoTracking()
            .Include(x => x.Districts)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (city is null)
            throw new NotFoundException("City not found.");

        return _mapper.Map<GetByIdCityResponse>(city);
    }


    public async Task UpdateCityAsync(UpdateCityRequest request, CancellationToken ct = default)
    {
        if (request is null)
            throw new BadRequestException("Request is null.");

        var validation = await _updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            throw new BadRequestException("Validation failed.", ToErrorList(validation));

        var entity = await _repository.GetByIdAsync(request.Id, ct);
        if (entity is null)
            throw new NotFoundException("City not found.");

        var trimmed = (request.Name ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
            throw new BadRequestException("Name is required.");

        // store formatted (First letter uppercase, rest lowercase)
        var formattedName = char.ToUpperInvariant(trimmed[0]) + trimmed[1..].ToLowerInvariant();

        var normalizedName = formattedName.ToLowerInvariant();

        var exists = await _repository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Id != request.Id && x.Name.ToLower() == normalizedName, ct);

        if (exists)
            throw new BadRequestException("City with this name already exists.");

        request.Name = formattedName;

        _mapper.Map(request, entity);

        _repository.Update(entity);
        await _repository.SaveChangesAsync(ct);
    }




    public async Task DeleteCityAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0)
            throw new BadRequestException("Invalid id.");

        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            throw new NotFoundException("City not found.");

        _repository.Delete(entity);
        await _repository.SaveChangesAsync(ct);
    }
}