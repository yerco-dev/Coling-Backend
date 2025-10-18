using Coling.Domain.Constants;
using Coling.Domain.Entities.AcademicManagement;
using Coling.Domain.Entities.Base;
using Coling.Domain.Entities.WorkManagement;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coling.Domain.Entities;

public class Member : BaseEntity
{
    [Required]
    [DataType(DataType.Date)]
    public DateTime MembershipDate { get; set; }

    [Required]
    [StringLength(50)]
    public string MembershipCode { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string TitleNumber { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = BusinessConstants.MemberStatusValues[MemberStatus.Pending];

    [Required]
    public Guid PersonId { get; set; }

    public Person? Person { get; set; }

    public ICollection<MemberEducation> Educations { get; set; } = new List<MemberEducation>();

    public ICollection<WorkExperience> WorkExperiences { get; set; } = new List<WorkExperience>();
}
