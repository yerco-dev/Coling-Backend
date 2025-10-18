using System.ComponentModel.DataAnnotations;

namespace Coling.Application.DTOs.InstitutionManagement;

public class RegisterInstitutionTypeDto
{
    [Required(ErrorMessage = "El nombre del tipo de institución es requerido.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
    public string Name { get; set; } = null!;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres.")]
    public string? Description { get; set; }
}
