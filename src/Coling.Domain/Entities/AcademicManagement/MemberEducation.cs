using Coling.Domain.Constants;
using Coling.Domain.Entities.Base;
using Coling.Domain.Entities.PartialDateManagement;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coling.Domain.Entities.AcademicManagement;

public class MemberEducation : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string TitleReceived { get; set; } = null!;

    public PartialDate? StartDate { get; set; }

    public PartialDate? EndDate { get; set; }

    [Url]
    [StringLength(500)]
    public string? DocumentUrl { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = BusinessConstants.EducationStatusValues[EducationStatus.Completed];

    [Required]
    public Guid MemberId { get; set; }

    [Required]
    public Guid EducationId { get; set; }

    [NotMapped]
    public bool IsInProgress => EndDate == null;

    public Member? Member { get; set; }
    public Education? Education { get; set; }
}
