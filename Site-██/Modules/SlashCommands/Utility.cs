using Discord.Interactions;
using Discord.WebSocket;
using Site___.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site___.Modules.SlashCommands
{
    public class Utility : InteractionModuleBase<ShardedInteractionContext>
    {
        [SlashCommand("close", "Closes the current ticket")]
        public async Task CloseTicket()
        {
            if (Context.Channel.GetType() != typeof(SocketThreadChannel))
            {
                await RespondAsync("This isnt a ticket, what are you trying to do?");
                return;
            }
            var Channel = Context.Channel as SocketThreadChannel;
            if (Channel.ParentChannel.Id != 873708122605228072)
            {
                await RespondAsync("Thread not under correct category");
                return;
            }

            await RespondAsync("Closed ticket", ephemeral: true);
            await Channel.ModifyAsync(x=>x.Archived = true);
            ActionLog.Send($"**CLOSED TICKET** {Channel.Mention}", Context.User);
        }
    }
}
