using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.PropertyAd;
using Application.Shared.Helpers.Exceptions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;


namespace Persistence.Services;

public sealed class PropertyAdService : IPropertyAdService
{
    private readonly IPropertyAdRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePropertyAdRequest> _createValidator;
    private readonly IValidator<UpdatePropertyAdRequest> _updateValidator;

    public PropertyAdService(
        IPropertyAdRepository repository,
        IMapper mapper,
        IValidator<CreatePropertyAdRequest> createValidator,
        IValidator<UpdatePropertyAdRequest> updateValidator)
    {
        _repository = repository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    private static List<string> ToErrorList(ValidationResult result)
        => result.Errors.Select(e => e.ErrorMessage).Distinct().ToList();

    public async Task CreatePropertyAdAsync(CreatePropertyAdRequest request, CancellationToken ct = default)
    {
        if (request is null)
            throw new BadRequestException("Request is null.");

        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            throw new BadRequestException("Validation failed.", ToErrorList(validation));

        var entity = _mapper.Map<PropertyAd>(request);

        await _repository.AddAsync(entity, ct);
        await _repository.SaveChangesAsync(ct);
    }

    public async Task<List<GetALLPropertyAdResponse>> GetAllPropertyAdsAsync(CancellationToken ct = default)
    {
                return await _repository.Query()
         .AsNoTracking()
         .OrderByDescending(x => x.Id)
         .ProjectTo<GetALLPropertyAdResponse>(_mapper.ConfigurationProvider)
         .ToListAsync(ct);


    }

    public async Task<GetByIdPropertyAdResponse> GetPropertyAdByIdAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0)
            throw new BadRequestException("Invalid id.");

        var entity = await _repository.Query()
            .AsNoTracking()
            .Include(x => x.Media)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (entity is null)
            throw new NotFoundException("PropertyAd not found.");

        return _mapper.Map<GetByIdPropertyAdResponse>(entity);
    }

    public async Task UpdatePropertyAdAsync(UpdatePropertyAdRequest request, CancellationToken ct = default)
    {
        if (request is null)
            throw new BadRequestException("Request is null.");

        var validation = await _updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            throw new BadRequestException("Validation failed.", ToErrorList(validation));

        var entity = await _repository.GetByIdAsync(request.Id, ct);
        if (entity is null)
            throw new NotFoundException("PropertyAd not found.");

        _mapper.Map(request, entity);

        _repository.Update(entity);
        await _repository.SaveChangesAsync(ct);
    }

    public async Task DeletePropertyAdAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0)
            throw new BadRequestException("Invalid id.");

        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            throw new NotFoundException("PropertyAd not found.");

        _repository.Delete(entity);
        await _repository.SaveChangesAsync(ct);
    }

    public async Task<List<GetALLPropertyAdResponse>> GetPropertyAdsByCategoryAsync(PropertyCategory category, CancellationToken ct = default)
    {
            return await _repository.Query()
         .AsNoTracking()
         .Where(x => x.PropertyCategory == category)
         .OrderByDescending(x => x.Id)
         .ProjectTo<GetALLPropertyAdResponse>(_mapper.ConfigurationProvider)
         .ToListAsync(ct);

    }
}