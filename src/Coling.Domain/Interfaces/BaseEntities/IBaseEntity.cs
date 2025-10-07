using Coling.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Coling.Domain.Interfaces.BaseEntities;

public interface IBaseEntity
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public bool IsActive { get; set; }
}
