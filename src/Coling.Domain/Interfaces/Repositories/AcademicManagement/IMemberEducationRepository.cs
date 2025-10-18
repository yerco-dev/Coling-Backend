using Coling.Domain.Entities.AcademicManagement;
using Coling.Domain.Interfaces.Repositories.Generics;
using Coling.Domain.Wrappers;

namespace Coling.Domain.Interfaces.Repositories.AcademicManagement;

public interface IMemberEducationRepository : IGenericRepository<MemberEducation>
{
    Task<ActionResponse<IEnumerable<MemberEducation>>> GetByMemberIdAsync(Guid memberId);
    Task<ActionResponse<IEnumerable<MemberEducation>>> GetByEducationIdAsync(Guid educationId);
    Task<ActionResponse<IEnumerable<MemberEducation>>> GetByMemberIdWithDetailsAsync(Guid memberId);
    Task<ActionResponse<IEnumerable<MemberEducation>>> GetInProgressByMemberIdAsync(Guid memberId);
    Task<ActionResponse<IEnumerable<MemberEducation>>> GetCompletedByMemberIdAsync(Guid memberId);
}
