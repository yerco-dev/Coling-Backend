namespace Coling.Aplication.DTOs.MembersManagement;

public class MemberDetailsDto
{
    public Guid MemberId { get; set; }
    public Guid UserId { get; set; }
    public Guid PersonId { get; set; }

    // Datos del usuario
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsActive { get; set; }

    // Datos personales
    public string NationalId { get; set; } = null!;
    public string FirstNames { get; set; } = null!;
    public string? PaternalLastName { get; set; }
    public string MaternalLastName { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public DateTime? BirthDate { get; set; }
    public string? PhotoUrl { get; set; }

    // Datos de membresía
    public DateTime MembershipDate { get; set; }
    public string MembershipCode { get; set; } = null!;
    public string TitleNumber { get; set; } = null!;
    public string Status { get; set; } = null!;
}
