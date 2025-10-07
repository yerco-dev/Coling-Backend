namespace Coling.Aplication.DTOs.UsersManagement;

public class LoginResponseDto
{
    public Guid UserId { get; set; }
    public Guid PersonId { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FirstNames { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string Token { get; set; } = null!;
}
