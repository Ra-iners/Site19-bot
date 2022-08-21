using Discord.Interactions;
using Discord.WebSocket;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Site___.Core
{
    internal class SlashCommandHandler
    {
        private readonly DiscordShardedClient _client;
        private readonly InteractionService _commands;
        public SlashCommandHandler(DiscordShardedClient Client, InteractionService Commands)
        {
            _client = Client;
            _commands = Commands;
        }

        public async Task InstallCommands()
        {
            _client.SlashCommandExecuted += HandleSlashCommand;
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
        }

        private async Task HandleSlashCommand(SocketSlashCommand arg)
        {
            var SlashCommandContext = new ShardedInteractionContext(_client, arg);
            await _commands.ExecuteCommandAsync(SlashCommandContext, null);
        }
    }
}
