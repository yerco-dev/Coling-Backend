using Coling.Domain.Entities.Base;
using Coling.Domain.Entities.InstitutionManagement;
using System.ComponentModel.DataAnnotations;

namespace Coling.Domain.Entities.AcademicManagement;

public abstract class Education : NamedEntity
{
    [Required]
    public Guid InstitutionId { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public Institution? Institution { get; set; }
}
