using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site___.Modules.SlashCommands
{
    public class Moderation : InteractionModuleBase<ShardedInteractionContext>
    {
        [SlashCommand("kick", "Kicks the person from the server", runMode:RunMode.Async)]
        public async Task Kick(IUser User, string Reason)
        {

        }
    }
}
