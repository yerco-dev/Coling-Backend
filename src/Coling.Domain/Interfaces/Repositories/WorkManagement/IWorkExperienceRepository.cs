using Coling.Domain.Entities.WorkManagement;
using Coling.Domain.Interfaces.Repositories.Generics;
using Coling.Domain.Wrappers;

namespace Coling.Domain.Interfaces.Repositories.WorkManagement;

public interface IWorkExperienceRepository : IGenericRepository<WorkExperience>
{
    Task<ActionResponse<IEnumerable<WorkExperience>>> GetByMemberIdAsync(Guid memberId);
    Task<ActionResponse<IEnumerable<WorkExperience>>> GetByInstitutionIdAsync(Guid institutionId);
    Task<ActionResponse<IEnumerable<WorkExperience>>> GetByMemberIdWithDetailsAsync(Guid memberId);
    Task<ActionResponse<IEnumerable<WorkExperience>>> GetCurrentJobsByMemberIdAsync(Guid memberId);
}
