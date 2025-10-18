using Coling.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Coling.Domain.Entities.WorkManagement;

public class WorkField : NamedEntity
{
    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public Guid WorkFieldCategoryId { get; set; }

    public WorkFieldCategory? WorkFieldCategory { get; set; }

    public ICollection<WorkExperienceField> WorkExperienceFields { get; set; } = new List<WorkExperienceField>();
}
