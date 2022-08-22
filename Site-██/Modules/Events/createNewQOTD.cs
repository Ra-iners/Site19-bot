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
    internal class createNewQOTD
    {
        public createNewQOTD()
        {
            Globals.Client.MessageReceived += onUserMessage;
        }

        // This is a weird way of doing it but hey it works
        DateTimeOffset QOTDCooldown = new DateTimeOffset();
        ulong QOTDMain = 755393387988779078;
        ulong QOTDSub = 846241073852055594;
        private async Task onUserMessage(SocketMessage arg)
        {
            if(DateTimeOffset.UtcNow > QOTDCooldown)
            {
                QOTDCooldown = DateTimeOffset.UtcNow.AddHours(24);
                await ForceQOTD();
            }
        }
        public async Task ForceQOTD()
        {
            ITextChannel QOTDMainC = Globals.Client.GetChannel(QOTDMain) as ITextChannel;
            ITextChannel QOTDSubC = Globals.Client.GetChannel(QOTDSub) as ITextChannel;

            List<IMessage> Possible = new List<IMessage>();
            foreach (var msg in await QOTDSubC.GetMessagesAsync(100).FlattenAsync())
                if (!msg.Author.IsBot && !msg.Author.IsWebhook) // ignore bots (i.e level-up messages)
                    Possible.Add(msg);

            IMessage rnd = Possible[new Random().Next(Possible.Count)];

            EmbedBuilder eb = new EmbedBuilder();
            eb.WithAuthor(rnd.Author);
            eb.WithDescription(rnd.Content);
            eb.WithFooter("Question of the day");
            eb.WithCurrentTimestamp();
            eb.WithColor(231, 77, 60);

            if (rnd.Attachments.Count > 0)
                eb.WithImageUrl(rnd.Attachments.First().Url);

            var Sent = await QOTDMainC.SendMessageAsync("<@&847573204623949904>", embed: eb.Build());
            await QOTDMainC.CreateThreadAsync("QOTD", message: Sent);
            ActionLog.Send("Created new QOTD");
        }
    }
}
