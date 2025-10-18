using Coling.Domain.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coling.Domain.Entities.AcademicManagement;

public class DegreeEducation : Education
{
    [Required]
    [StringLength(50)]
    public string AcademicDegree { get; set; } = BusinessConstants.AcademicDegreeValues[Constants.AcademicDegree.Licentiate];

    [Required]
    [StringLength(200)]
    public string Major { get; set; } = null!;

    [StringLength(100)]
    public string? Specialization { get; set; }

    [StringLength(200)]
    public string? ThesisTitle { get; set; }

    [Range(0, 100)]
    [Column(TypeName = "decimal(5,2)")]
    public decimal? GPA { get; set; }

    public bool HasHonors { get; set; }
}
