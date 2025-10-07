using System.ComponentModel.DataAnnotations;

namespace Coling.Aplication.DTOs.UsersManagement;

public class LoginRequestDto
{
    [Required(ErrorMessage = "El nombre de usuario es requerido.")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es requerida.")]
    public string Password { get; set; } = null!;
}
