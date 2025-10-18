using Coling.Domain.Entities.Base;
using Coling.Domain.Entities.PartyManagement;
using Coling.Domain.Entities.WorkManagement;
using Coling.Domain.Interfaces.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace Coling.Domain.Entities.InstitutionManagement;

public class Institution : Party, INamedEntity
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    public Guid InstitutionTypeId { get; set; }

    public InstitutionType? InstitutionType { get; set; } = null!;

    public ICollection<WorkExperience> WorkExperiences { get; set; } = new List<WorkExperience>();
}
