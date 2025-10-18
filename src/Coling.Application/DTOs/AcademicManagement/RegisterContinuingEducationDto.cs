using System.ComponentModel.DataAnnotations;

namespace Coling.Application.DTOs.AcademicManagement;

public class RegisterContinuingEducationDto
{
    [Required(ErrorMessage = "El ID de la institución es requerido.")]
    public Guid InstitutionId { get; set; }

    [Required(ErrorMessage = "El nombre es requerido.")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres.")]
    public string Name { get; set; } = null!;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres.")]
    public string? Description { get; set; }

    [Range(1, 10000, ErrorMessage = "La duración debe estar entre 1 y 10000 horas.")]
    public int? DurationHours { get; set; }

    [Required(ErrorMessage = "El tipo de educación es requerido.")]
    [StringLength(50, ErrorMessage = "El tipo de educación no puede exceder 50 caracteres.")]
    public string EducationType { get; set; } = null!;

    public bool IssuesCertificate { get; set; }

    [StringLength(100, ErrorMessage = "El número de certificado no puede exceder 100 caracteres.")]
    public string? CertificateNumber { get; set; }

    [Required(ErrorMessage = "El título recibido es requerido.")]
    [StringLength(200, ErrorMessage = "El título recibido no puede exceder 200 caracteres.")]
    public string TitleReceived { get; set; } = null!;

    public int? StartYear { get; set; }
    public int? StartMonth { get; set; }
    public int? StartDay { get; set; }

    public int? EndYear { get; set; }
    public int? EndMonth { get; set; }
    public int? EndDay { get; set; }

    [Required(ErrorMessage = "El estado es requerido.")]
    [StringLength(50, ErrorMessage = "El estado no puede exceder 50 caracteres.")]
    public string Status { get; set; } = null!;
}
