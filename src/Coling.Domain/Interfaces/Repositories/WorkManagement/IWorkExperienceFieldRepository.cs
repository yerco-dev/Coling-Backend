using Coling.Domain.Entities.WorkManagement;
using Coling.Domain.Interfaces.Repositories.Generics;
using Coling.Domain.Wrappers;

namespace Coling.Domain.Interfaces.Repositories.WorkManagement;

public interface IWorkExperienceFieldRepository : IGenericRepository<WorkExperienceField>
{
    Task<ActionResponse<IEnumerable<WorkExperienceField>>> GetByWorkExperienceIdAsync(Guid workExperienceId);
    Task<ActionResponse<IEnumerable<WorkExperienceField>>> GetByWorkFieldIdAsync(Guid workFieldId);
}
