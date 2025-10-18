using Coling.Domain.Entities.AcademicManagement;
using Coling.Domain.Interfaces.Repositories.Generics;
using Coling.Domain.Wrappers;

namespace Coling.Domain.Interfaces.Repositories.AcademicManagement;

public interface IContinuingEducationRepository : IGenericRepository<ContinuingEducation>
{
    Task<ActionResponse<IEnumerable<ContinuingEducation>>> GetByEducationTypeAsync(string educationType);
    Task<ActionResponse<IEnumerable<ContinuingEducation>>> GetWithCertificateAsync();
}
