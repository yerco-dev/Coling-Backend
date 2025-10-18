namespace Coling.Application.DTOs.AcademicManagement;

public class ProfessionalCertificationDetailDto
{
    public Guid MemberEducationId { get; set; }
    public Guid EducationId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public Guid InstitutionId { get; set; }
    public string InstitutionName { get; set; } = null!;
    public string CertificationNumber { get; set; } = null!;
    public DateTime? ExpirationDate { get; set; }
    public bool RequiresRenewal { get; set; }
    public string TitleReceived { get; set; } = null!;
    public int? StartYear { get; set; }
    public int? StartMonth { get; set; }
    public int? StartDay { get; set; }
    public int? EndYear { get; set; }
    public int? EndMonth { get; set; }
    public int? EndDay { get; set; }
    public string? DocumentUrl { get; set; }
    public string Status { get; set; } = null!;
    public bool IsActive { get; set; }
}
