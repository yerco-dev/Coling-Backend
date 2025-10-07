using Coling.Domain.Entities;
using Coling.Domain.Interfaces.Repositories.PeopleManagement;
using Coling.Infrastructure.Data;
using Coling.Infrastructure.Repositories.Generics;

namespace Coling.Infrastructure.Repositories.PeopleManagement;

public class PersonRepository : GenericRepository<Person>, IPersonRepository
{
    public PersonRepository(AppDbContext context) : base(context)
    {

    }
}