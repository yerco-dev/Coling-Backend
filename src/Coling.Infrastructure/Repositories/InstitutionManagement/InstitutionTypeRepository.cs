using Coling.Domain.Entities.InstitutionManagement;
using Coling.Domain.Interfaces.Repositories.InstitutionManagement;
using Coling.Infrastructure.Data;
using Coling.Infrastructure.Repositories.Generics;

namespace Coling.Infrastructure.Repositories.InstitutionManagement;

public class InstitutionTypeRepository : GenericRepository<InstitutionType>, IInstitutionTypeRepository
{
    public InstitutionTypeRepository(AppDbContext context) : base(context)
    {
    }
}
