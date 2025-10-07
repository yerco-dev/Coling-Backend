using System.ComponentModel.DataAnnotations;

namespace Coling.Aplication.DTOs.UsersManagement;

public class RegisterMemberUserDto
{
    [Required]
    [StringLength(256)]
    public string UserName { get; set; } = null!;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]+$",
        ErrorMessage = "La contraseña debe contener al menos una mayúscula, una minúscula, un número y un carácter especial.")]
    public string Password { get; set; } = null!;

    [Required]
    [StringLength(20)]
    public string NationalId { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string FirstNames { get; set; } = null!;

    [MaxLength(100)]
    public string? PaternalLastName { get; set; }

    [Required]
    [MaxLength(100)]
    public string MaternalLastName { get; set; } = null!;

    [Required]
    public DateTime MembershipDate { get; set; }

    [Required]
    [StringLength(50)]
    public string MembershipCode { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string TitleNumber { get; set; } = null!;
}
