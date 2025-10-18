using Coling.Domain.Entities.AcademicManagement;
using Coling.Domain.Interfaces.Repositories.Generics;
using Coling.Domain.Wrappers;

namespace Coling.Domain.Interfaces.Repositories.AcademicManagement;

public interface IProfessionalCertificationRepository : IGenericRepository<ProfessionalCertification>
{
    Task<ActionResponse<IEnumerable<ProfessionalCertification>>> GetExpiringCertificationsAsync(DateTime beforeDate);
    Task<ActionResponse<IEnumerable<ProfessionalCertification>>> GetByCertificationNumberAsync(string certificationNumber);
}
