namespace Coling.Aplication.DTOs.UsersManagement;

public class UserGetDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public bool isActive { get; set; }
}
