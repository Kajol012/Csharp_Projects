using System.Data.SqlClient;

namespace CampusEventManagement
{
    // Base class: defines a common access-policy contract for CRUD modules.
    public abstract class ModuleAccessPolicy
    {
        public abstract string GetSelectFilter(string ownerColumn);
        public abstract string GetMutationFilter(string ownerColumn);
        public abstract SqlParameter[] GetOwnerParameters(string ownerColumn);
    }

    // Admin modules can view and manage all records.
    public class AdminModuleAccessPolicy : ModuleAccessPolicy
    {
        public override string GetSelectFilter(string ownerColumn)
        {
            return "";
        }

        public override string GetMutationFilter(string ownerColumn)
        {
            return "";
        }

        public override SqlParameter[] GetOwnerParameters(string ownerColumn)
        {
            return new SqlParameter[0];
        }
    }

    // Non-admin modules can only access records created by the signed-in user.
    public class OwnerModuleAccessPolicy : ModuleAccessPolicy
    {
        public override string GetSelectFilter(string ownerColumn)
        {
            return " WHERE " + ownerColumn + "=@owner";
        }

        public override string GetMutationFilter(string ownerColumn)
        {
            return " AND " + ownerColumn + "=@owner";
        }

        public override SqlParameter[] GetOwnerParameters(string ownerColumn)
        {
            return new[] { Db.P("@owner", Session.UserId) };
        }
    }
}
