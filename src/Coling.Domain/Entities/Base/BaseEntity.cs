using Coling.Domain.Interfaces.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace Coling.Domain.Entities.Base;

public class BaseEntity : IBaseEntity
{
    [Key]
    public virtual Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public virtual bool IsActive { get; set; } = true;
}
