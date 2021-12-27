using Discord.Commands;
using DiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class GoogleModule : ModuleBase
    {
        public GoogleService GoogleService { get; set; }
        public Settings.Deserialized.Settings Settings { get; set; }

        [Command("google"), Alias("lmgtfy"), Priority(100)]
        [Summary("Creates a google link with the query. Syntax: !google cute kittens")]
        public async Task Google(string searchTerm)
        {
            string defaultLink = "https://lmgtfy.app/?q=";
            searchTerm = searchTerm.Replace(" ", "+");
            string link = defaultLink + searchTerm;
        }
    }
}
