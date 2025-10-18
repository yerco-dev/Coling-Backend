using System.ComponentModel.DataAnnotations;

namespace Coling.Application.DTOs.AcademicManagement;

public class RegisterProfessionalCertificationDto
{
    [Required(ErrorMessage = "El ID de la institución es requerido.")]
    public Guid InstitutionId { get; set; }

    [Required(ErrorMessage = "El nombre es requerido.")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres.")]
    public string Name { get; set; } = null!;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "El número de certificación es requerido.")]
    [StringLength(100, ErrorMessage = "El número de certificación no puede exceder 100 caracteres.")]
    public string CertificationNumber { get; set; } = null!;

    [DataType(DataType.Date)]
    public DateTime? ExpirationDate { get; set; }

    public bool RequiresRenewal { get; set; }

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
