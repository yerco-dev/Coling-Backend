using System.ComponentModel.DataAnnotations;

namespace Coling.Domain.Entities.AcademicManagement;

public class ProfessionalCertification : Education
{
    [Required]
    [StringLength(100)]
    public string CertificationNumber { get; set; } = null!;

    [DataType(DataType.Date)]
    public DateTime? ExpirationDate { get; set; }

    public bool RequiresRenewal { get; set; }
}
