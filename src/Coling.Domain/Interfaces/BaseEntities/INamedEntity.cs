using System.ComponentModel.DataAnnotations;

namespace Coling.Domain.Interfaces.BaseEntities;

internal interface INamedEntity : IBaseEntity
{
    [Required]
    public string Name { get; set; }
}
