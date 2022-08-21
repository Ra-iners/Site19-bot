using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site___.Core
{
    internal class Globals
    {
        public static DiscordShardedClient Client;
        public static IServiceProvider ServiceProvider { get; set; }
        public static CommandService CommandService;
        public static InteractionService InteractionService;
        public static JObject Config = (JObject)JsonConvert.DeserializeObject(File.ReadAllText("Configuration.json"));
    }
}
