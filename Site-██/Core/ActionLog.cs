using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Webhook;

namespace Site___.Core
{
    internal static class ActionLog
    {
        static DiscordWebhookClient Webhook = new DiscordWebhookClient(Globals.Config["LOG_WEBHOOK"].ToString());
        public static async void Send(string Reason, string Avatar=null, string Name=null)
        {
            await Webhook.SendMessageAsync(Reason, username: Name, avatarUrl: Avatar);
        }
        public static async void Send(string Reason, IUser Caller)
        {
            await Webhook.SendMessageAsync(Reason, username: $"{Caller}", avatarUrl: Caller.GetAvatarUrl()??Caller.GetDefaultAvatarUrl());
        }
    }
}
