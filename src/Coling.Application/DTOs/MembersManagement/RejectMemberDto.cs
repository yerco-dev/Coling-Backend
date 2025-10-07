using System.ComponentModel.DataAnnotations;

namespace Coling.Aplication.DTOs.MembersManagement;

public class RejectMemberDto
{
    [Required(ErrorMessage = "El ID del usuario es requerido.")]
    public Guid UserId { get; set; }

    [StringLength(500, ErrorMessage = "El motivo no puede exceder 500 caracteres.")]
    public string? Reason { get; set; }
}
