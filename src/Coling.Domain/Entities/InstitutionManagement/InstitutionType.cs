using Coling.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Coling.Domain.Entities.InstitutionManagement;

public class InstitutionType : NamedEntity
{
    public ICollection<Institution> Institutions { get; set; } = new List<Institution>();
}
