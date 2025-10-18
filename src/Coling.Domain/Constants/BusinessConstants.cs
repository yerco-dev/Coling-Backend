namespace Coling.Domain.Constants;

public enum MemberStatus
{
    Pending,
    Verified,
    Suspended,
}
public enum SystemRoles
{
    Admin,
    Member,
    Moderator,
}

public enum EducationStatus
{
    InProgress,
    Completed,
    DroppedOut,
}

public enum AcademicDegree
{
    BachelorDegree,
    Licentiate,
    Diploma,
    Specialty,
    Master,
    Doctorate,
    Postdoctorate,
}

public enum ContinuingEducationType
{
    Course,
    Workshop,
    Seminar,
    Conference,
}


public class BusinessConstants
{
    public static bool IsValidConstant<TEnum>(string value, Dictionary<TEnum, string> constants) where TEnum : struct, Enum
    {
        return constants.Values.Contains(value);
    }
    public static TEnum? ParseConstant<TEnum>(string value, Dictionary<TEnum, string> constants) where TEnum : struct, Enum
    {
        var entry = constants.FirstOrDefault(x => x.Value == value);
        return entry.Key.Equals(default(TEnum)) ? null : entry.Key;
    }

    public static readonly Dictionary<MemberStatus, string> MemberStatusValues  = new()
    {
        { MemberStatus.Pending, "Pendiente de Verificación" },
        { MemberStatus.Verified, "Verificado" },
        { MemberStatus.Suspended, "Suspendido" }
    };
    public static bool IsValidMemberStatus(string status) =>
        IsValidConstant(status, MemberStatusValues);

    public static MemberStatus? ParseMemberStatus(string status) =>
        ParseConstant(status, MemberStatusValues);


    public static readonly Dictionary<SystemRoles, string> SystemRolesValues = new()
    {
        { SystemRoles.Admin, "Administrador" },
        { SystemRoles.Member, "Miembro" },
        { SystemRoles.Moderator, "Moderador" }
    };

    public static bool IsValidSystemRole(string role) =>
        IsValidConstant(role, SystemRolesValues);

    public static SystemRoles? ParseSystemRole(string role) =>
        ParseConstant(role, SystemRolesValues);

    public static readonly Dictionary<EducationStatus, string> EducationStatusValues = new()
    {
        { EducationStatus.InProgress, "En progreso" },
        { EducationStatus.Completed, "Completado" },
        { EducationStatus.DroppedOut, "Abandonado" }
    };
    public static bool IsValidEducationStatus(string status) =>
        IsValidConstant(status, EducationStatusValues);

    public static EducationStatus? ParseEducationStatus(string status) =>
        ParseConstant(status, EducationStatusValues);

    public static readonly Dictionary<AcademicDegree, string> AcademicDegreeValues = new()
    {
        { AcademicDegree.BachelorDegree, "Bachiller" },
        { AcademicDegree.Licentiate, "Licenciatura" },
        { AcademicDegree.Diploma, "Diplomado" },
        { AcademicDegree.Specialty, "Especialidad" },
        { AcademicDegree.Master, "Maestría" },
        { AcademicDegree.Doctorate, "Doctorado" },
        { AcademicDegree.Postdoctorate, "Postdoctorado" }
    };

    public static bool IsValidAcademicDegree(string degree) =>
        IsValidConstant(degree, AcademicDegreeValues);

    public static AcademicDegree? ParseAcademicDegree(string degree) =>
        ParseConstant(degree, AcademicDegreeValues);

    public static readonly Dictionary<ContinuingEducationType, string> ContinuingEducationTypeValues = new()
    {
        { ContinuingEducationType.Course, "Curso" },
        { ContinuingEducationType.Workshop, "Taller" },
        { ContinuingEducationType.Seminar, "Seminario" },
        { ContinuingEducationType.Conference, "Conferencia" }
    };

    public static bool IsValidContinuingEducationType(string type) =>
        IsValidConstant(type, ContinuingEducationTypeValues);

    public static ContinuingEducationType? ParseContinuingEducationType(string type) =>
        ParseConstant(type, ContinuingEducationTypeValues);
}
