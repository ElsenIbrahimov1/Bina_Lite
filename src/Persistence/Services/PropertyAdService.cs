using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.PropertyAd;
using Application.Shared.Helpers.Responses;
using AutoMapper;
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

    public async Task<BaseResponse> CreatePropertyAdAsync(CreatePropertyAdRequest request, CancellationToken ct = default)
    {
        if (request is null)
            return BaseResponse.Fail("Request is null.", 400);

        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return BaseResponse.Fail("Validation failed.", 400, ToErrorList(validation));

        var entity = _mapper.Map<PropertyAd>(request);

        await _repository.AddAsync(entity, ct);
        await _repository.SaveChangesAsync(ct);

        return BaseResponse.Ok("Created.", 201);
    }

    public async Task<BaseResponse<List<GetALLPropertyAdResponse>>> GetAllPropertyAdsAsync(CancellationToken ct = default)
    {
        var data = await _repository.Query()
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .Select(x => _mapper.Map<GetALLPropertyAdResponse>(x))
            .ToListAsync(ct);

        return BaseResponse<List<GetALLPropertyAdResponse>>.Ok(data);
    }

    public async Task<BaseResponse<GetByIdPropertyAdResponse>> GetPropertyAdByIdAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0)
            return BaseResponse<GetByIdPropertyAdResponse>.Fail("Invalid id.", 400);

        var entity = await _repository.Query()
            .AsNoTracking()
            .Include(x => x.Media)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (entity is null)
            return BaseResponse<GetByIdPropertyAdResponse>.Fail("PropertyAd not found.", 404);

        var dto = _mapper.Map<GetByIdPropertyAdResponse>(entity);
        return BaseResponse<GetByIdPropertyAdResponse>.Ok(dto);
    }

    public async Task<BaseResponse> UpdatePropertyAdAsync(UpdatePropertyAdRequest request, CancellationToken ct = default)
    {
        if (request is null)
            return BaseResponse.Fail("Request is null.", 400);

        var validation = await _updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return BaseResponse.Fail("Validation failed.", 400, ToErrorList(validation));

        var entity = await _repository.GetByIdAsync(request.Id, ct);
        if (entity is null)
            return BaseResponse.Fail("PropertyAd not found.", 404);

        _mapper.Map(request, entity);

        _repository.Update(entity);
        await _repository.SaveChangesAsync(ct);

        return BaseResponse.Ok("Updated.", 200);
    }

    public async Task<BaseResponse> DeletePropertyAdAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0)
            return BaseResponse.Fail("Invalid id.", 400);

        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return BaseResponse.Fail("PropertyAd not found.", 404);

        _repository.Delete(entity);
        await _repository.SaveChangesAsync(ct);

        return BaseResponse.Ok("Deleted.", 200);
    }

    public async Task<BaseResponse<List<GetALLPropertyAdResponse>>> GetPropertyAdsByCategoryAsync(PropertyCategory category, CancellationToken ct = default)
    {
        var data = await _repository.Query()
            .AsNoTracking()
            .Where(x => x.PropertyCategory == category)
            .OrderByDescending(x => x.Id)
            .Select(x => _mapper.Map<GetALLPropertyAdResponse>(x))
            .ToListAsync(ct);

        return BaseResponse<List<GetALLPropertyAdResponse>>.Ok(data);
    }
}