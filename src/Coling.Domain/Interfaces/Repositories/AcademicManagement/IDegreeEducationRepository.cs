using Coling.Domain.Entities.AcademicManagement;
using Coling.Domain.Interfaces.Repositories.Generics;
using Coling.Domain.Wrappers;

namespace Coling.Domain.Interfaces.Repositories.AcademicManagement;

public interface IDegreeEducationRepository : IGenericRepository<DegreeEducation>
{
    Task<ActionResponse<IEnumerable<DegreeEducation>>> GetByAcademicDegreeAsync(string academicDegree);
    Task<ActionResponse<IEnumerable<DegreeEducation>>> GetByMajorAsync(string major);
}
