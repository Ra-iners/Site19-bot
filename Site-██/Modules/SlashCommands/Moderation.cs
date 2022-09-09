using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using Site___.Core;
using Site___.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site___.Modules.SlashCommands
{
    public class Moderation : InteractionModuleBase<ShardedInteractionContext>
    {
        [SlashCommand("kick", "Kicks the person from the server", runMode:RunMode.Async)]
        public async Task Kick(IGuildUser User, string Reason)
        {
            await Context.Interaction.DeferAsync();
            var Self = Context.Guild.GetUser(Context.Client.CurrentUser.Id);
            if(!(Context.User as IGuildUser).HasPermission(GuildPermission.KickMembers))
            {
                await FollowupAsync("Missing permissions, please contact Rai if you think this is a mistake.");
                return;
            }
            if(Self.Hierarchy <= User.Hierarchy)
            {
                await FollowupAsync($"Users hierarchy is higher than mine, I am unable to take action.");
                return;
            }

            try
            {
                Reason = Reason.ParseRules();
                EmbedBuilder e = new EmbedBuilder();
                e.WithTitle(":tools: Kicked!");
                e.WithDescription($"You have been kicked from **{Context.Guild.Name}**");
                e.AddField("Reason", $"```{Reason}```");
                e.WithColor(Color.Orange);
                e.WithFooter($"Kicked by: {Context.User.Username}#{Context.User.Discriminator}", Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl());
                await User.SendMessageAsync(embed: e.Build());
            }
            catch
            {
                await Context.Channel.SendMessageAsync("I was unable to DM the user the kick reason");
            }

            await User.KickAsync($"{Context.User.Username}#{Context.User.Discriminator}: {Reason}");
            await FollowupAsync("User was kicked from the server");
                
            ActionLog.Send($"**KICKED** {User.Username}#{User.Discriminator}: ``{Reason}``", Context.User);
        }

        [SlashCommand("ban", "Bans the person from the server", runMode: RunMode.Async)]
        public async Task Ban(IGuildUser User, string Reason, int DaysToPurge=0)
        {
            await Context.Interaction.DeferAsync();
            var Self = Context.Guild.GetUser(Context.Client.CurrentUser.Id);
            if (!(Context.User as IGuildUser).HasPermission(GuildPermission.BanMembers))
            {
                await FollowupAsync("Missing permissions, please contact Rai if you think this is a mistake.");
                return;
            }
            if (Self.Hierarchy <= User.Hierarchy)
            {
                await FollowupAsync($"Users hierarchy is higher than mine, I am unable to take action.");
                return;
            }

            try
            {
                Reason = Reason.ParseRules();
                EmbedBuilder e = new EmbedBuilder();
                e.WithTitle(":tools: Banned!");
                e.WithDescription($"You have been banned from **{Context.Guild.Name}**");
                e.AddField("Reason", $"```{Reason}```");
                e.WithColor(Color.Red);
                e.WithFooter($"Banned by: {Context.User.Username}#{Context.User.Discriminator}", Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl());
                await User.SendMessageAsync(embed: e.Build());
            }
            catch
            {
                await Context.Channel.SendMessageAsync("I was unable to DM the user the ban reason");
            }

            await User.BanAsync(DaysToPurge, $"{Context.User.Username}#{Context.User.Discriminator}: {Reason}");
            await FollowupAsync("User was banned from the server");

            ActionLog.Send($"**BANNED** {User.Username}#{User.Discriminator}: ``{Reason}``", Context.User);
        }
        [SlashCommand("sban", "soft-bans a person (bans and then unbans, removing their messages)")]
        public async Task sban(IGuildUser User, string Reason, int DaysToPurge)
        {
            await Context.Interaction.DeferAsync();
            var Self = Context.Guild.GetUser(Context.Client.CurrentUser.Id);
            if (!(Context.User as IGuildUser).HasPermission(GuildPermission.KickMembers))
            {
                await FollowupAsync("Missing permissions, please contact Rai if you think this is a mistake.");
                return;
            }
            if (Self.Hierarchy <= User.Hierarchy)
            {
                await FollowupAsync($"Users hierarchy is higher than mine, I am unable to take action.");
                return;
            }

            try
            {
                Reason = Reason.ParseRules();
                EmbedBuilder e = new EmbedBuilder();
                e.WithTitle(":tools: Soft-Banned!");
                e.WithDescription($"You have been banned from **{Context.Guild.Name}**");
                e.AddField("Reason", $"```{Reason}```");
                e.AddField("Note", "A soft-ban is similar to a kick, you are able to rejoin, however, a few of your past messages have now been deleted");
                e.WithColor(Color.Red);
                e.WithFooter($"Soft-Banned by: {Context.User.Username}#{Context.User.Discriminator}", Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl());
                await User.SendMessageAsync(embed: e.Build());
            }
            catch
            {
                await Context.Channel.SendMessageAsync("I was unable to DM the user the ban reason");
            }

            await User.BanAsync(DaysToPurge, $"{Context.User.Username}#{Context.User.Discriminator}: {Reason}");
            await Context.Guild.RemoveBanAsync(User.Id);
            await FollowupAsync("User was soft-banned from the server");

            ActionLog.Send($"**SOFT-BANNED** {User.Username}#{User.Discriminator}: ``{Reason}`` {DaysToPurge} days worth of messages were purged", Context.User);
        }

        [SlashCommand("unban", "Unbans the given user from the server", runMode: RunMode.Async)]
        public async Task Unban(string Username, string Reason)
        {
            await Context.Interaction.DeferAsync();
            if (!(Context.User as IGuildUser).HasPermission(GuildPermission.BanMembers))
            {
                await FollowupAsync("Missing permissions, please contact Rai if you think this is a mistake.");
                return;
            }
            Reason = Reason.ParseRules();

            List<RestBan> Matches = new List<RestBan>();
            foreach (var Banned in await Context.Guild.GetBansAsync(5000).FlattenAsync())
            {
                Username = Username.ToLower();
                string name = $"{Banned.User.Username}#{Banned.User.Discriminator}".ToLower();

                if (name.Contains(Username))
                {
                    Matches.Add(Banned);
                }
            }
            if (Matches.Count > 1)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var match in Matches)
                    sb.Append($"``{match.User.Username}#{match.User.Discriminator}``, ");
                await FollowupAsync($"Multiple matches found: {sb}");
            }
            else if (Matches.Count == 1)
            {
                var Person = Matches.First();
                await Context.Guild.RemoveBanAsync(Person.User.Id, new RequestOptions()
                {
                    AuditLogReason = $"Unbanned user | By: {Context.User.Username}#{Context.User.Discriminator}"
                });
                await FollowupAsync($"✅ Successfully unbanned **{Person}**");
                ActionLog.Send($"**UNBANNED** {Person}: {Reason}", Context.User);
            }
            else
            {
                await FollowupAsync("🟥 No users with the given name were found");
            }
        }

        private static Random random = new Random();
        public static string randomString(int length)
        {
            const string chars = "abcdef123456790";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());}

        [SlashCommand("warn", "Gives the specified user a warning, if target is staff, then gives them a strike instead", runMode: RunMode.Async)]
        public async Task warn(IGuildUser User, string Reason)
        {
            await Context.Interaction.DeferAsync();
            if (!(Context.User as IGuildUser).HasPermission(GuildPermission.ModerateMembers))
            {
                await FollowupAsync("Missing permissions, please contact Rai if you think this is a mistake.");
                return;
            }

            try
            {
                Reason = Reason.ParseRules();
                EmbedBuilder e = new EmbedBuilder();
                e.WithTitle($"Warning received!");
                e.WithDescription($"You have received a warning in **{Context.Guild.Name}**");
                e.AddField("Reason", $"```{Reason}```");
                e.WithColor(Color.Orange);
                e.WithFooter($"Warned by: {Context.User}", Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl());
                await User.SendMessageAsync(embed: e.Build());
            }
            catch
            {
                await Context.Channel.SendMessageAsync("I was unable to DM the user the warning reason");
            }

            string id = randomString(7);
            User.AddWarning(id, Context.User, Reason);
            await FollowupAsync("Successfully warned user");
            ActionLog.Send($"**WARNED** {User} for the following reason: ```{Reason}```", Context.User);
        }

        [SlashCommand("warnings", "Gets the list of warnings that the user has", runMode: RunMode.Async)]
        public async Task Warnings(IGuildUser User)
        {
            await Context.Interaction.DeferAsync();
            if (!(Context.User as IGuildUser).HasPermission(GuildPermission.ModerateMembers))
            {
                await FollowupAsync("Missing permissions, please contact Rai if you think this is a mistake.");
                return;
            }

            var Warnings = User.Warnings();
            if (Warnings.Count() == 0)
            {
                await FollowupAsync("This user has no warnings");
                return;
            }
            EmbedBuilder e = new EmbedBuilder();
            e.WithTitle($"Warnings for {User}");
            e.WithColor(Color.Orange);
            foreach (var Warning in Warnings)
            {
                EmbedFieldBuilder Field = new EmbedFieldBuilder();
                Field.WithIsInline(true);
                if (DateTimeOffset.Now.ToUnixTimeSeconds() <= Warning.Expires)
                    Field.WithName($"🟥 {Warning.ID}"); // Red square
                else
                    Field.WithName($"🟩 {Warning.ID}"); // Green square

                Field.WithValue($@"Warned By: <@{Warning.Caller}>
Reason: {Warning.Reason}");
                e.AddField(Field);
            }
            await FollowupAsync(embeds: new Embed[] { e.Build() });
            ActionLog.Send($"**VIEWED WARNINGS** for {User}", Context.User);
        }
        [SlashCommand("delwarn", "Deletes the given warning", runMode: RunMode.Async)]
        public async Task DeleteWarning(IUser User, [Summary("WarnID", "You can find this via /warnings command")] string WarningID)
        {
            await Context.Interaction.DeferAsync();
            if (!(Context.User as IGuildUser).HasPermission(GuildPermission.ModerateMembers))
            {
                await FollowupAsync("Missing permissions, please contact Rai if you think this is a mistake.");
                return;
            }

            bool wasDeleted = User.DeleteWarning(WarningID);
            if (wasDeleted)
            {
                await FollowupAsync("Warning successfully deleted");
                ActionLog.Send($"**DELETED WARNING**: for **{User}** via ID ``{WarningID}``", Context.User);
            }
            else
                await FollowupAsync("Warning couldn't be deleted, does it exist?");
        }
        public enum TimeFormat
        {
            Minutes,
            Hours,
            Days,
            Weeks
        }
        [SlashCommand("mute", "Mutes the given user for the specified time", runMode:RunMode.Async)]
        public async Task Mute(IGuildUser User, string Reason, int Days=0, int Hours=0, int Minutes=0, bool shouldWarn=true)
        {
            await Context.Interaction.DeferAsync();
            if (!(Context.User as IGuildUser).HasPermission(GuildPermission.ModerateMembers))
            {
                await FollowupAsync("Missing permissions, please contact Rai if you think this is a mistake.");
                return;
            }

            try
            {
                Reason = Reason.ParseRules();
                if (Days==0&&Hours==0&&Minutes==0)
                {
                    await FollowupAsync("Atleast 1 time format (days, hours, minutes) is needed for this command");
                    return;
                }
                TimeSpan ts = new TimeSpan(Days, Hours, Minutes, 0);
                DateTimeOffset dto= new DateTimeOffset(DateTime.Now) + ts;
                long unix = dto.ToUnixTimeSeconds();

                try
                {
                    EmbedBuilder e = new EmbedBuilder();
                    e.WithTitle(":tools: Muted!");
                    e.WithDescription($"You have been muted in **{Context.Guild.Name}**");
                    e.AddField("Reason", $"```{Reason}```");
                    e.AddField("Mute Duration", $"<t:{unix}:F>");
                    e.WithColor(Color.Red);
                    e.WithFooter($"Muted by: {Context.User.Username}#{Context.User.Discriminator}", Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl());
                    await User.SendMessageAsync(embed: e.Build());
                }
                catch
                {
                    await Context.Channel.SendMessageAsync("I was unable to DM the user the mute reason");
                }

                await User.SetTimeOutAsync(ts);
                string warnId = "NONE";
                if (shouldWarn)
                {
                    // Create a warning for the user
                    warnId = "M-"+randomString(7); // M- is the prefix for mute-warnings
                    User.AddWarning(warnId, Context.User, Reason);
                }
                await FollowupAsync($"User was successfully muted until <t:{unix}:F>");
                ActionLog.Send($"**MUTED** {User} until <t:{unix}:F>. Reason: ``{Reason}``\nA warning has also automatically been attached with this mute with the id ``{warnId}``", Context.User);
            }
            catch(Exception ex)
            {
                await FollowupAsync(ex.ToString());
            }
        }
        [SlashCommand("unmute", "Unmutes the given user", runMode:RunMode.Async)]
        public async Task UnMute(IGuildUser User, [Summary("Reason","Why are you unmuting this person?")]string Reason)
        {
            await Context.Interaction.DeferAsync();
            if (!(Context.User as IGuildUser).HasPermission(GuildPermission.ModerateMembers))
            {
                await FollowupAsync("Missing permissions, please contact Rai if you think this is a mistake.");
                return;
            }
            Reason = Reason.ParseRules();
            if (User.TimedOutUntil > DateTime.UtcNow || User.TimedOutUntil != null)
            {
                await User.RemoveTimeOutAsync();
                await FollowupAsync("User was unmuted successfully");
                ActionLog.Send($"**UNMUTED** {User} Reason: ``{Reason}``", Context.User);
            }
            else
            {
                await FollowupAsync("User isn't muted!");
            }
        }
        public enum PurgeFilter
        {
            [ChoiceDisplay("New Members (below 1 day of joining)")]
            NewMembers,
            Bots,
            Webhooks,
            [ChoiceDisplay("Bots & Webhooks")]
            BotsnWebhooks,
            Users,
            [ChoiceDisplay("All (default)")]
            All,
            [ChoiceDisplay("Files (removes only messages with images/files)")]
            Files,
            [ChoiceDisplay("URL's (removes messages with url's in them)")]
            URLS,
            [ChoiceDisplay("Embed's (messages with embeds in them)")]
            Embeds
        }
        [SlashCommand("purge", "Purges a set amount of messages from the given channel")]
        public async Task Purge(int Amount, ITextChannel Channel=null,PurgeFilter Filter=PurgeFilter.All)
        {
            await Context.Interaction.DeferAsync(true);
            if (!(Context.User as IGuildUser).HasPermission(GuildPermission.ManageMessages))
            {
                await FollowupAsync("Missing permissions, please contact Rai if you think this is a mistake.");
                return;
            }
            Channel ??= Context.Channel as ITextChannel;
            var Messages = await Channel.GetMessagesAsync(Amount).FlattenAsync();

            List<IMessage> toPurge = new List<IMessage>();
            foreach (var msg in Messages)
            {
                if (Filter == PurgeFilter.NewMembers)
                {
                    IGuildUser person = msg.Author as IGuildUser;
                    if(person.JoinedAt != null || person.JoinedAt < DateTimeOffset.UtcNow.AddDays(1))
                        toPurge.Add(msg);
                }
                if (Filter == PurgeFilter.Bots)
                    if (msg.Author.IsBot)
                        toPurge.Add(msg);
                if (Filter == PurgeFilter.Webhooks)
                    if (msg.Author.IsWebhook)
                        toPurge.Add(msg);
                if (Filter == PurgeFilter.BotsnWebhooks)
                    if (msg.Author.IsWebhook || msg.Author.IsBot)
                        toPurge.Add(msg);
                if (Filter == PurgeFilter.Users)
                    if (!msg.Author.IsBot && !msg.Author.IsWebhook)
                        toPurge.Add(msg);
                if (Filter == PurgeFilter.All)
                    toPurge.Add(msg);
                if (Filter == PurgeFilter.Files)
                    if (msg.Attachments.Count > 0)
                        toPurge.Add(msg);
                if (Filter == PurgeFilter.URLS)
                    if (msg.Content.Contains("http"))
                        toPurge.Add(msg);
                if (Filter == PurgeFilter.Embeds)
                    if (msg.Embeds.Count > 0)
                        toPurge.Add(msg);
            }

            StringBuilder SB = new StringBuilder();
            foreach(var msg in toPurge)
            {
                SB.AppendLine($"{msg.Author}: {msg.Content}");
                foreach(var att in msg.Attachments)
                    SB.AppendLine($"Attachment: {att.Url}");
                SB.AppendLine();
                
            }
            File.WriteAllText("purgeLog.txt", SB.ToString());

            try
            {
                var HOS = Context.Client.GetChannel(966827718626918460) as ITextChannel;
                await HOS.SendFileAsync("purgeLog.txt", $"{Context.User.Mention} in {Channel.Mention}");
            }
            catch
            {
                ActionLog.Send("Failed saving log", incrimentCase:false);
            }

            try
            {
                

                await Channel.DeleteMessagesAsync(toPurge);
                await FollowupAsync("Done!");
                var msg = await Context.Channel.SendMessageAsync($"Deleted {toPurge.Count} messages with the filter ``{Filter}``");
                ActionLog.Send($"**PURGE** deleted {toPurge.Count} messages (out of requested {Amount}) with the filter ``{Filter}`` in {Channel.Mention}", Context.User);
                await Task.Delay(5000);
                await msg.DeleteAsync();
            }
            catch(Exception ex)
            {
                await Context.Channel.SendMessageAsync(ex.ToString());
            }
        }
    }
}
