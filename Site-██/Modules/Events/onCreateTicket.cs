using Serilog;
using Site___.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;

namespace Site___.Modules.Events
{
    internal class onCreateTicket
    {
        public onCreateTicket()
        {
            Globals.Client.SelectMenuExecuted += Client_SelectMenuExecuted;
        }

        private async Task Client_SelectMenuExecuted(SocketMessageComponent ctx)
        {
            if (ctx.Data.CustomId != "ticket_reason") return;
            await ctx.DeferAsync(true);
            string res = ctx.Data.Values.FirstOrDefault();

            var Guild = Globals.Client.GetGuild(696437457620828250);
            var TicketChannel = Guild.GetChannel(873708122605228072) as ITextChannel;
            // Create a new ticket thread

            var caseId = ActionLog.getCaseId();
            var Thread = await TicketChannel.CreateThreadAsync($"Ticket {caseId}", ThreadType.PrivateThread, ThreadArchiveDuration.ThreeDays);

            var e = new EmbedBuilder();
            e.WithTitle($":tickets: Support Request");
            e.WithColor(47, 49, 54);
            if (res == "rb")
                e.AddField("Reason", "Rule Breaker");
            if (res == "gq")
                e.AddField("Reason", "General Question");
            if (res == "o")
                e.AddField("Reason", "Other");


            await Thread.SendMessageAsync($"Hello {ctx.User.Mention}, a <@&790157462007185408> member will assist you shortly, please explain your issue in more detail", embed: e.Build());
            await ctx.FollowupAsync($"A ticket was created in {Thread.Mention}",ephemeral:true);
        }
    }
}
