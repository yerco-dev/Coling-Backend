using System.ComponentModel.DataAnnotations;

namespace Coling.Aplication.DTOs.MembersManagement;

public class ApproveMemberDto
{
    [Required(ErrorMessage = "El ID del usuario es requerido.")]
    public Guid UserId { get; set; }
}
