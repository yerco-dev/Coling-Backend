using Coling.Domain.Constants;
using Coling.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coling.Domain.Entities;

public class Member : BaseEntity
{
    [Required]
    public DateTime MembershipDate { get; set; }

    [Required]
    [StringLength(50)]
    public string MembershipCode { get; set; } = null!;

    [StringLength(100)]
    public string TitleNumber { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = BusinessConstants.MemberStatusValues[MemberStatus.Pending];

    [Required]
    public Guid PersonId { get; set; }

    public Person? Person { get; set; }
}
