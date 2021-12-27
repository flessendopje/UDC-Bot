using Discord.WebSocket;
using DiscordBot.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    public class GoogleService
    {
        private readonly DiscordSocketClient client;
        private readonly ILoggingService loggingService;
        private bool isRunning;
        public GoogleService(DiscordSocketClient client, ILoggingService loggingService, Settings.Deserialized.Settings settings)
        {
            this.client = client;
            this.loggingService = loggingService;
            isRunning = false;

            client.Ready += OnReady;
        }

        private async Task OnReady()
        {
            if (!isRunning)
            {
                isRunning = true;
                await loggingService.LogAction("Started LMGTFY Service");
                await Listen();
            }
        }

        private Task Listen()
        {
            while (isRunning)
            {

            }
            return null;
        }
    }
}
