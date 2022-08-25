using System;
using System.Collections.Generic;
using System.IO;
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
        public static async void Send(string Reason, string Avatar=null, string Name=null, bool incrimentCase=true)
        {
            int CaseID = getCaseId();
            await Webhook.SendMessageAsync($"**CASE {CaseID}** | {Reason}", username: Name, avatarUrl: Avatar);
            if (incrimentCase)
                setCaseNumber(CaseID+1);
        }
        public static async void Send(string Reason, IUser Caller, bool incrimentCase=true) =>
            Send(Reason, Caller.GetAvatarUrl() ?? Caller.GetDefaultAvatarUrl(), $"{Caller}", incrimentCase);

        public static int getCaseId()
        {
            int CurrentID = int.Parse(File.ReadAllText("Database/CaseID.txt"));
            return CurrentID;
        }
        // public static void incrimentCaseID()=>File.WriteAllText("Database/CaseID.txt", (getCaseId() + 1).ToString());
        public static void setCaseNumber(int num) => File.WriteAllText("Database/CaseID.txt", num.ToString());
    }
}
