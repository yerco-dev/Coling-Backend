using System.ComponentModel.DataAnnotations;

namespace Coling.Application.DTOs.WorkManagement;

public class UpdateWorkExperienceDto
{
    [Required]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "El ID de la institución es requerido.")]
    public Guid InstitutionId { get; set; }

    [Required(ErrorMessage = "El título del puesto es requerido.")]
    [StringLength(150, ErrorMessage = "El título no puede exceder 150 caracteres.")]
    public string JobTitle { get; set; } = null!;

    [Required(ErrorMessage = "La fecha de inicio es requerida.")]
    public int? StartYear { get; set; }
    public int? StartMonth { get; set; }
    public int? StartDay { get; set; }

    public int? EndYear { get; set; }
    public int? EndMonth { get; set; }
    public int? EndDay { get; set; }

    [StringLength(2000, ErrorMessage = "La descripción no puede exceder 2000 caracteres.")]
    public string? Description { get; set; }

    [StringLength(1000, ErrorMessage = "Las responsabilidades no pueden exceder 1000 caracteres.")]
    public string? Responsibilities { get; set; }

    [StringLength(1000, ErrorMessage = "Los logros no pueden exceder 1000 caracteres.")]
    public string? Achievements { get; set; }

    [Required(ErrorMessage = "Debe seleccionar al menos un campo de trabajo.")]
    public List<Guid> WorkFieldIds { get; set; } = new List<Guid>();
}
