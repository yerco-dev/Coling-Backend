using Coling.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Coling.Domain.Entities.WorkManagement;

public class WorkExperienceField : BaseEntity
{
    [Required]
    public Guid WorkExperienceId { get; set; }

    [Required]
    public Guid WorkFieldId { get; set; }

    public WorkExperience? WorkExperience { get; set; }
    public WorkField? WorkField { get; set; }
}
