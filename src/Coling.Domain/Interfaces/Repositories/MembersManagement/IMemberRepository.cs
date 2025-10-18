using Coling.Domain.Entities;
using Coling.Domain.Wrappers;
using Coling.Domain.Interfaces.Repositories.Generics;

namespace Coling.Domain.Interfaces.Repositories.MembersManagement;

public interface IMemberRepository : IGenericRepository<Member>
{
    Task<ActionResponse<IEnumerable<Member>>> GetPendingMembersAsync();
    Task<ActionResponse<Member>> GetMemberByPersonIdAsync(Guid personId);
}
