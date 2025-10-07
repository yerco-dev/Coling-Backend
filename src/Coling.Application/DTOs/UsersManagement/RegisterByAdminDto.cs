using System.ComponentModel.DataAnnotations;

namespace Coling.Aplication.DTOs.UsersManagement;

public class RegisterByAdminDto
{
    public string UserName { get; set; } = null!;
    [EmailAddress]
    public string Email { get; set; } = null!;
    public string FirstNames { get; set; } = null!;
    public string? PaternalLastName { get; set; }
    public string MaternalLastName { get; set; } = null!;
    public string Role { get; set; } = null!;
}

