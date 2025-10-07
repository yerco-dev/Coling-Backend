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

public class BusinessConstants
{
    public static readonly Dictionary<MemberStatus, string> MemberStatusValues  = new()
    {
        { MemberStatus.Pending, "Pendiente de Verificación" },
        { MemberStatus.Verified, "Verificado" },
        { MemberStatus.Suspended, "Suspendido" }
    };

    public static readonly Dictionary<SystemRoles, string> SystemRolesValues = new()
    {
        { SystemRoles.Admin, "Administrador" },
        { SystemRoles.Member, "Miembro" },
        { SystemRoles.Moderator, "Moderador" }
    };

    public static bool IsValidMemberStatus(string status) =>
        MemberStatusValues.Values.Contains(status);

    public static MemberStatus? ParseMemberStatus(string status)
    {
        var entry = MemberStatusValues.FirstOrDefault(x => x.Value == status);
        return entry.Key != default ? entry.Key : null;
    }

    public static bool IsValidSystemRole(string role) =>
        SystemRolesValues.Values.Contains(role);

    public static SystemRoles? ParseSystemRole(string role)
    {
        var entry = SystemRolesValues.FirstOrDefault(x => x.Value == role);
        return entry.Key != default ? entry.Key : null;
    }
}
