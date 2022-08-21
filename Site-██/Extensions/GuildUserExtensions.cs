using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site___.Extensions
{
    public static class GuildUserExtensions
    {
        public static bool HasPermission(this IGuildUser User, GuildPermission Permission)
        {
            if (User.GuildPermissions.Has(Permission) || User.GuildPermissions.Has(GuildPermission.Administrator))
                return true;
            return false;
        }
    }
}
