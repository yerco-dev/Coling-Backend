using System.ComponentModel.DataAnnotations;

namespace Coling.Application.DTOs.InstitutionManagement;

public class UpdateInstitutionTypeDto
{
    [Required(ErrorMessage = "El nombre del tipo de institución es requerido.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
    public string Name { get; set; } = null!;
}
