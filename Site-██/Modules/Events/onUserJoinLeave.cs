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
    internal class onUserJoinLeave
    {
        ITextChannel JoinLeave;
        public onUserJoinLeave()
        {
            Globals.Client.UserJoined += onUserJoin;
            Globals.Client.UserLeft += onUserLeave;
            JoinLeave = Globals.Client.GetChannel(696437457633148952) as ITextChannel;
        }

        private async Task onUserLeave(SocketGuild g, SocketUser user)
        {
            EmbedBuilder eb = new EmbedBuilder();
            eb.WithAuthor(user);
            eb.WithDescription($":wave: Goodbye **{user}**..\nWas nice having you here.\n\nRegistered: <t:{user.CreatedAt.ToUnixTimeSeconds()}:R>");
            eb.WithColor(Color.Red);
            eb.WithFooter(user.Id.ToString());

            await JoinLeave.SendMessageAsync(embed: eb.Build());
        }

        private async Task onUserJoin(SocketGuildUser user)
        {
            EmbedBuilder eb = new EmbedBuilder();
            eb.WithAuthor(user);
            eb.WithDescription($":wave: Welcome **{user}**!\nWe hope you enjoy your stay.\n\nRegistered: <t:{user.CreatedAt.ToUnixTimeSeconds()}:R>");
            eb.WithColor(Color.Green);
            eb.WithFooter(user.Id.ToString());

            await JoinLeave.SendMessageAsync(embed: eb.Build());
        }
    }
}
