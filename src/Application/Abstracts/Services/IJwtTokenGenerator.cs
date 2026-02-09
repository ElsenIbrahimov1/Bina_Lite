using Domain.Entities;

namespace Application.Abstracts.Services;

public interface IJwtTokenGenerator
{
    string GenerateToken(AppUser user);
}
