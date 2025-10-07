namespace Coling.Aplication.DTOs.UsersManagement;

public class UserLogedDataDto
{
    public Guid Id {  get; set; }
    public Guid PersonId { get; set; }
    public string Role { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string FirstNames { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Token { get; set; } = null!;
}
