using Coling.Domain.Entities.Base;
using Coling.Domain.Interfaces.BaseEntities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Coling.Domain.Entities.UsersManagement;

public class Role : IdentityRole<Guid>, INamedEntity
{
    [Required]
    public override string Name { get; set; } = null!;

    [Required]
    public bool IsActive { get; set; } = true;

    [MaxLength(250)]
    public string? Description { get; set; }
}