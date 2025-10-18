using Coling.Domain.Entities.AcademicManagement;
using Coling.Domain.Entities.Base;
using Coling.Domain.Entities.InstitutionManagement;
using Coling.Domain.Entities.PartialDateManagement;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coling.Domain.Entities.WorkManagement;

public class WorkExperience : BaseEntity
{
    [Required]
    public Guid MemberId { get; set; }

    [Required]
    public Guid InstitutionId { get; set; }

    [Required]
    [StringLength(150)]
    public string JobTitle { get; set; } = null!;

    [Required]
    public PartialDate StartDate { get; set; } = null!;

    public PartialDate? EndDate { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    [StringLength(1000)]
    public string? Responsibilities { get; set; }

    [StringLength(1000)]
    public string? Achievements { get; set; }

    [Url]
    [StringLength(500)]
    public string? DocumentUrl { get; set; }

    public Member? Member { get; set; } = null!;
    public Institution? Institution { get; set; } = null!;

    public ICollection<WorkExperienceField> WorkExperienceFields { get; set; } = new List<WorkExperienceField>();

    [NotMapped]
    public int DurationInMonths
    {
        get
        {
            var end = EndDate?.ToApproximateDate() ?? DateTime.Now;
            var start = StartDate.ToApproximateDate();
            var totalMonths = ((end.Year - start.Year) * 12) + end.Month - start.Month;
            return totalMonths > 0 ? totalMonths : 0;
        }
    }
}
