using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Site___.Core;
using Site___.Extensions;
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
        public static DateTimeOffset ActivityCooldown = DateTimeOffset.UtcNow;
        [SlashCommand("activity-ping", "Makes the chat wake up")]
        public async Task ActivityPing()
        {
            SocketGuildUser Caller = Context.User as SocketGuildUser;
            IRole Level20 = Context.Guild.Roles.First(x => x.Id == 758316485831032852);
            if(!Caller.Roles.Contains(Level20))
            {
                await RespondAsync("You need to be atleast level 20 to be able to use this command");
                return;
            }

            if(ActivityCooldown < DateTimeOffset.UtcNow)
            {
                await Context.Channel.SendMessageAsync($"<@&828263416488787998> invoked by {Context.User}");
                await RespondAsync("Pinged!");

                ActivityCooldown = DateTimeOffset.UtcNow.AddHours(12);
            }
            else
            {
                await RespondAsync($"Currently on cooldown, next ping will be available in <t:{ActivityCooldown.ToUnixTimeSeconds()}:R>");
            }
        }
        public enum InternalCommand
        {
            create_ticket_embed,
            force_qotd
        }
        [SlashCommand("internal", "Internal commands, just some stuff that doesn't have a category")]
        public async Task internalCommands(InternalCommand cmd)
        {
            
            if (!(Context.User as IGuildUser).HasPermission(GuildPermission.Administrator)) return;

            await Context.Interaction.DeferAsync();
            
            try
            {
                if (cmd == InternalCommand.create_ticket_embed)
                {
                    try
                    {
                        EmbedBuilder gui = new EmbedBuilder();
                        gui.WithTitle($":tickets: Create a new ticket");
                        gui.WithColor(Color.Red);
                        gui.WithDescription($"Got a rule breaker to report or maybe and issue your having? then you may use this to report on your problem/idea");
                        gui.WithFooter("Notice: selecting one of the options will instantly open a new ticket");

                        ComponentBuilder cb = new ComponentBuilder();
                        cb.WithSelectMenu(new SelectMenuBuilder()
                        {
                            CustomId = "ticket_reason",
                            Placeholder = "Select a reason",
                            Options = new List<SelectMenuOptionBuilder>()
                        {
                            new SelectMenuOptionBuilder()
                            {
                                Label = "Rule Breaker",
                                Description = "Someone's breaking the rules? Report them via this",
                                Emote = Emoji.Parse(":tools:"),
                                Value = "rb"
                            },
                            new SelectMenuOptionBuilder()
                            {
                                Label = "General Question",
                                Description = "Got a question that a regular person cannot answer? Ask it here!",
                                Emote = Emoji.Parse(":thinking:"),
                                Value = "gq"
                            },
                            new SelectMenuOptionBuilder()
                            {
                                Label = "Other",
                                Description = "Reason not listed? Use this instead",
                                Emote = Emoji.Parse(":regional_indicator_q:"),
                                Value = "o"
                            },

                        }
                        });
                        await Context.Channel.SendMessageAsync(embed: gui.Build(), components: cb.Build());
                        await FollowupAsync("Ticket GUI created", ephemeral: true);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                if(cmd == InternalCommand.force_qotd)
                {
                    await Globals.QOTD.ForceQOTD();
                    await FollowupAsync("Done!");
                }
            }
            catch(Exception ex)
            {
                await RespondAsync(ex.ToString());
            }
        }
    }
}
