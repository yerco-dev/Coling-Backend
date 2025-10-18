using System.ComponentModel.DataAnnotations;

namespace Coling.Application.DTOs.InstitutionManagement;

public class UpdateInstitutionDto
{
    [Required(ErrorMessage = "El nombre de la institución es requerido.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "El tipo de institución es requerido.")]
    public Guid InstitutionTypeId { get; set; }
}
