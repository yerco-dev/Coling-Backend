using Coling.Domain.Interfaces.BaseEntities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coling.Domain.Entities.UsersManagement;

public class User : IdentityUser<Guid>, IBaseEntity
{
    [Required]
    public Guid PersonId { get; set; }

    [ForeignKey(nameof(PersonId))]
    public Person Person { get; set; } = null!;

    [Required]
    public bool IsActive { get; set; } = true;
}
