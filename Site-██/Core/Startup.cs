using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site___.Core
{
    public class Startup
    {
        public async Task MainAsync()
        {
            Globals.Client = new DiscordShardedClient(new DiscordSocketConfig()
            {
                AlwaysDownloadDefaultStickers = true,
                AlwaysDownloadUsers = true,
                AlwaysResolveStickers = true,
                MaxWaitBetweenGuildAvailablesBeforeReady = 60000,
                MessageCacheSize = 4096,
                LogLevel = LogSeverity.Info,
                GatewayIntents = GatewayIntents.All,
                TotalShards = null, // Discord gets the recommended shard amount and uses that
            });
            

            Globals.CommandService = new CommandService();
            Globals.ServiceProvider = new ServiceCollection()
                .AddSingleton(Globals.Client)
                .AddSingleton(Globals.CommandService)
                .BuildServiceProvider();
            Globals.Client.Log += Client_Log;

            await Globals.Client.LoginAsync(TokenType.Bot, Globals.Config["BOT_TOKEN"].ToString());
            await Globals.Client.StartAsync();

            Globals.Client.ShardReady += Client_ShardReady;

            await Task.Delay(-1); // Wait until program is closed
        }

        private async Task Client_Log(LogMessage msg)
        {
            var severity = msg.Severity switch
            {
                LogSeverity.Critical => LogEventLevel.Fatal,
                LogSeverity.Error => LogEventLevel.Error,
                LogSeverity.Warning => LogEventLevel.Warning,
                LogSeverity.Info => LogEventLevel.Information,
                LogSeverity.Verbose => LogEventLevel.Verbose,
                LogSeverity.Debug => LogEventLevel.Debug,
                _ => LogEventLevel.Information
            };
            Log.Write(severity, msg.Exception, "[{Source}] {Message}", msg.Source, msg.Message);
            await Task.CompletedTask;
        }
        static bool Loaded = false;
        private async Task Client_ShardReady(DiscordSocketClient arg)
        {
            if(!Loaded) // We're doing this since the ShardReady can be called several times (i.e bot reconnect)
            {
                Loaded = true;
                Globals.InteractionService = new InteractionService(arg.Rest);
                await new SlashCommandHandler(Globals.Client, Globals.InteractionService).InstallCommands();
                await Globals.InteractionService.RegisterCommandsToGuildAsync(696437457620828250, true);
            }
        }
    }
}
