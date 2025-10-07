using Coling.Domain.Entities.ActionResponse;
using Coling.Domain.Entities.UsersManagement;
using System.Security.Claims;

namespace Coling.Application.Interfaces.Services;

public interface ITokenService
{
    Task<ActionResponse<string>> GenerateTokenAsync(User user, IEnumerable<Claim> claims);
}
