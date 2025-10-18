namespace Coling.Application.DTOs.AcademicManagement;

public class ProfessionalCertificationCreatedDto
{
    public Guid MemberEducationId { get; set; }
    public Guid EducationId { get; set; }
    public string EducationName { get; set; } = null!;
    public string InstitutionName { get; set; } = null!;
    public string TitleReceived { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? DocumentUrl { get; set; }
}
