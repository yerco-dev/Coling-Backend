using Coling.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Coling.Domain.Entities.WorkManagement;

public class WorkFieldCategory : NamedEntity
{
    [StringLength(500)]
    public string? Description { get; set; }

    public ICollection<WorkField> WorkFields { get; set; } = new List<WorkField>();
}
