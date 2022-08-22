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
    internal class RaidWatch
    {
        public RaidWatch()
        {
            Globals.Client.UserJoined += onUserJoin;
        }

        int TimeFrame = 3; // in minutes, checks how many people have joined during that time span
        int Treshold = 4; // How many people are required to join in the given time frame for the bot to start alerting staff
        static List<IGuildUser> newMembers = new List<IGuildUser>();
        private async Task onUserJoin(SocketGuildUser arg)
        {
            newMembers.Add(arg);
            
            foreach (var user in newMembers)
            {
                if(user.JoinedAt < DateTimeOffset.Now.AddMinutes(TimeFrame) &&
                    user.JoinedAt > DateTimeOffset.Now.AddMinutes(-TimeFrame))
                {
                    // within time frame, do nothing
                }
                else
                {
                    newMembers.Remove(user);
                }
            }

            var Channel = Globals.Client.GetChannel(696437458400706675) as ITextChannel;
            if (newMembers.Count >= Treshold)
                await Channel.SendMessageAsync($"Possible raid, there have been **{newMembers.Count}** new members in the span of **{TimeFrame * 2}** minutes");
            else if(newMembers.Count >= 10)
                await Channel.SendMessageAsync($"<@&790157462007185408> Certain raid, there have been **{newMembers.Count}** new members in the span of **{TimeFrame * 2}** minutes");
        }
    }
}
