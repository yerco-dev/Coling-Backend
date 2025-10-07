using Coling.Domain.Interfaces.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace Coling.Domain.Entities.Base;

public class NamedEntity : BaseEntity, INamedEntity
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
}
