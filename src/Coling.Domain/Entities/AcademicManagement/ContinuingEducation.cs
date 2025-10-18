using Coling.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace Coling.Domain.Entities.AcademicManagement;

public class ContinuingEducation : Education
{
    [Range(1, 10000)]
    public int? DurationHours { get; set; }

    [Required]
    [StringLength(50)]
    public string EducationType { get; set; } = BusinessConstants.ContinuingEducationTypeValues[ContinuingEducationType.Course];

    public bool IssuesCertificate { get; set; }

    [StringLength(100)]
    public string? CertificateNumber { get; set; }
}
