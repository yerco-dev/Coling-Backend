using System.ComponentModel.DataAnnotations;

namespace Coling.Application.DTOs.AcademicManagement;

public class RegisterDegreeEducationDto
{
    [Required(ErrorMessage = "El ID de la institución es requerido.")]
    public Guid InstitutionId { get; set; }

    [Required(ErrorMessage = "El nombre del programa es requerido.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
    public string Name { get; set; } = null!;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "El grado académico es requerido.")]
    [StringLength(50)]
    public string AcademicDegree { get; set; } = null!;

    [Required(ErrorMessage = "La carrera es requerida.")]
    [StringLength(200)]
    public string Major { get; set; } = null!;

    [StringLength(100)]
    public string? Specialization { get; set; }

    [StringLength(200)]
    public string? ThesisTitle { get; set; }

    [Range(0, 100, ErrorMessage = "El GPA debe estar entre 0 y 100.")]
    public decimal? GPA { get; set; }

    public bool HasHonors { get; set; }

    [Required(ErrorMessage = "El título recibido es requerido.")]
    [StringLength(200)]
    public string TitleReceived { get; set; } = null!;

    public int? StartYear { get; set; }
    public int? StartMonth { get; set; }
    public int? StartDay { get; set; }

    public int? EndYear { get; set; }
    public int? EndMonth { get; set; }
    public int? EndDay { get; set; }

    [Required(ErrorMessage = "El estado es requerido.")]
    [StringLength(50)]
    public string Status { get; set; } = null!;
}
