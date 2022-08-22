using Site___.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Site___.Extensions;
using Discord;

namespace Site___.Modules.Events
{
    internal class React2Suggestions
    {
        public React2Suggestions()
        {
            Globals.Client.MessageReceived += Client_MessageReceived;
        }

        private async Task Client_MessageReceived(SocketMessage msg)
        {
            if (msg.Channel.Id != 811536427186913281) return;
            if (!(msg.Author as IGuildUser).HasPermission(GuildPermission.ModerateMembers)) return;

            if(!msg.Content.ToLower().Contains("suggestion") || !msg.Content.ToLower().Contains("description"))
            {
                await msg.DeleteAsync();
                var warning = await msg.Channel.SendMessageAsync($"{msg.Author.Mention} please use the format ```Suggestion:\nDescription:```\nto create a suggestion");
                await Task.Delay(2500);
                await warning.DeleteAsync();
                return;
            }
            IEmote Approve = Emote.Parse("<:approve:768054795332222986>");
            IEmote Deny = Emote.Parse("<:deny:768054428288417812>");

            await msg.AddReactionAsync(Approve);
            await msg.AddReactionAsync(Deny);
        }
    }
}
