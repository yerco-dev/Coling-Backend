using Coling.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coling.Domain.Entities;

public class Person : BaseEntity
{
    [Required]
    [StringLength(20)]
    public string NationalId { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string FirstNames { get; set; } = null!;

    [MaxLength(100)]
    public string? PaternalLastName { get; set; }

    [Required, MaxLength(100)]
    public string MaternalLastName { get; set; } = null!;

    public DateTime? BirthDate { get; set; }

    public string? PhotoUrl { get; set; }

    public Member? Member { get; set; }


    [NotMapped]
    public string FullName =>
        $"{FirstNames} {PaternalLastName} {MaternalLastName}".Trim();
}
