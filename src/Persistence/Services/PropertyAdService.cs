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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Persistence.Services;

public sealed class PropertyAdService : IPropertyAdService
{
    private readonly IPropertyAdRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePropertyAdRequest> _createValidator;
    private readonly IValidator<UpdatePropertyAdRequest> _updateValidator;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailSender _emailSender;

    public PropertyAdService(
    IPropertyAdRepository repository,
    IMapper mapper,
    IValidator<CreatePropertyAdRequest> createValidator,
    IValidator<UpdatePropertyAdRequest> updateValidator,
    UserManager<AppUser> userManager,
    IEmailSender emailSender)
    {
        _repository = repository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _userManager = userManager;
        _emailSender = emailSender;
    }

    private static List<string> ToErrorList(ValidationResult result)
        => result.Errors.Select(e => e.ErrorMessage).Distinct().ToList();

    public async Task CreatePropertyAdAsync(CreatePropertyAdRequest request, string userId, CancellationToken ct = default)
    {
        if (request is null)
            throw new BadRequestException("Request is null.");

        if (string.IsNullOrWhiteSpace(userId))
            throw new BadRequestException("UserId is required.");

        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            throw new BadRequestException("Validation failed.", ToErrorList(validation));

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            throw new NotFoundException("User not found.");

        // Map request -> entity
        var entity = _mapper.Map<PropertyAd>(request);

        // ✅ connect to user
        entity.CreatedByUserId = userId;

        await _repository.AddAsync(entity, ct);
        await _repository.SaveChangesAsync(ct);

        // ✅ Get details using your existing GetById mapping logic
        var created = await _repository.Query()
            .AsNoTracking()
            .Include(x => x.Media)
            .FirstOrDefaultAsync(x => x.Id == entity.Id, ct);

        // build response dto exactly like GetById
        var dto = _mapper.Map<GetByIdPropertyAdResponse>(created);

        // ✅ email content
        var subject = $"Your property ad was created (Id: {dto.Id})";

        var html = $"""
<p>Hello {user.FullName},</p>
<p>Your property ad has been created successfully.</p>

<h3>Property Ad Info</h3>
<ul>
  <li><b>Id:</b> {dto.Id}</li>
  <li><b>Title:</b> {dto.Title}</li>
  <li><b>Description:</b> {dto.Description}</li>
  <li><b>Price:</b> {dto.Price}</li>
  <li><b>Rooms:</b> {dto.RoomCount}</li>
  <li><b>Area:</b> {dto.AreaInSquareMeters}</li>
  <li><b>OfferType:</b> {dto.OfferType}</li>
  <li><b>Category:</b> {dto.PropertyCategory}</li>
</ul>
<p>You can view it via GET by id endpoint: <b>/api/propertyad/{dto.Id}</b></p>
""";

        var text = $"""
Hello {user.FullName},
Your property ad has been created successfully.

Id: {dto.Id}
Title: {dto.Title}
Description: {dto.Description}
Price: {dto.Price}
Rooms: {dto.RoomCount}
Area: {dto.AreaInSquareMeters}
OfferType: {dto.OfferType}
Category: {dto.PropertyCategory}

GET: /api/propertyad/{dto.Id}
""";

        // ✅ send email
        await _emailSender.SendAsync(user.Email ?? "", subject, html, text, ct);
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