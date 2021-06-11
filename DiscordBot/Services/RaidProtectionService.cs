using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DiscordBot.Services
{
    public class RaidProtectionService
    {
        public bool IsLockDownEnabled { get; private set; } = false;
        
        private readonly ILoggingService _loggingService;
        
        // Settings
        private readonly Settings.Deserialized.RaidProtection _raidSettings;

        private string _overridenKickMessage = string.Empty;
        private DateTime _overridenEndTime = DateTime.Now.AddSeconds(-30);

        private DateTime _lastJoinDate = DateTime.Now;
        private DateTime _raidStartTime;
        private int _usersInRaidCount;
        private List<SocketGuildUser> _usersInRaid = new List<SocketGuildUser>();
        private string _raidUsernameBlacklist = string.Empty;
        
        private SocketRole _moderatorRole;

        public RaidProtectionService(DiscordSocketClient client, ILoggingService logging,
            Settings.Deserialized.RaidProtection raidSettings, Settings.Deserialized.Settings settings)
        {
            _raidSettings = raidSettings;
            _loggingService = logging;

            // Event Subscriptions
            client.UserJoined += UserJoined;

            client.Ready += () =>
            {
                _moderatorRole = client.GetGuild(settings.GuildId).GetRole(settings.ModeratorRoleId);
                return Task.CompletedTask;
            };
        }

        private async Task UserJoined(SocketGuildUser user)
        {
            // If we're in manual override mode
            if (DateTime.Now < _overridenEndTime && IsLockDownEnabled)
            {
                await ProcessKick(user);
                return;
            }
            // Otherwise check if lastJoinDate is longer than the shutoff period
            else if ((DateTime.Now - _lastJoinDate).TotalSeconds > _raidSettings.MaxJoinSeconds)
            {
                await DisableLockdown();
                return;
            }
            await ProcessKick(user);
        }

        private async Task ProcessKick(SocketGuildUser user)
        {
            if (_raidUsernameBlacklist != string.Empty && !user.Username.ToLower().Contains(_raidUsernameBlacklist))
                return;
            
            // Add the current user to usersInRaid, increase _usersInRaid by 1 and update lastJoinDate to currentTime.
            _usersInRaidCount++;
            _usersInRaid.Add(user);
            _lastJoinDate = DateTime.Now;
            // Check the if the number of users inside usersInRaid is bigger than Y [joinMaxNewUsers]
            //      ==> If True, kick all users inside usersInRaid and remove them from the list as they are kicked.
            if (_usersInRaid.Count > _raidSettings.MaxNewUsers || IsLockDownEnabled)
            {
                if (IsLockDownEnabled == false)
                {
                    _raidStartTime = DateTime.Now;
                    await _loggingService.LogAction($"{_raidSettings.RaidProtectionIdentifier} {_moderatorRole.Mention} The server has automatically entered lockdown!");
                }
                // Since we need to reach a number before we start kicking, our first kick contains a group, afterwards we just kick them as they join to reduce odds of messaging users.
                IsLockDownEnabled = true;
                // spin up a new task so the GateWay event for this can finish and we don't get GateWay limited
                await Task.Run(async () => await CrudeRaidKicker(new List<SocketGuildUser>(_usersInRaid)));

                _usersInRaid.Clear();
            }
        }
        
        private async Task CrudeRaidKicker(List<SocketGuildUser> raiders)
        {
            for (int i = 0; i <= raiders.Count - 1; i++)
            {
                var raider = raiders[i];
                try
                {
                    // This can fail, so we have to catch that.
                    // We check if we have a custom message, otherwise give it the normal one.
                    if (_overridenKickMessage != string.Empty)
                        await raider.SendMessageAsync(_overridenKickMessage);
                    else
                        await raider.SendMessageAsync(_raidSettings.KickMessage);
                }
                catch (Exception)
                {
                    // await _loggingService.LogAction($"{raidProtectLine} Failed to notify user of kick {raider.user.Mention}", false);
                }
                if (_overridenKickMessage != string.Empty)
                    await raider.KickAsync(_overridenKickMessage);
                else
                    await raider.KickAsync(_raidSettings.KickMessage);
                
                await _loggingService.LogAction($"{_raidSettings.RaidProtectionIdentifier} {raider.Mention} has been kicked due to lockdown.");
            }
            raiders.Clear();
        }

        public async Task DisableLockdown()
        {
            if (IsLockDownEnabled && _usersInRaidCount > 0) {
                await _loggingService.LogAction(
                    $"{_raidSettings.RaidProtectionIdentifier} {_usersInRaidCount} users were kicked over {(DateTime.Now - _raidStartTime).Seconds} seconds before resuming regular operations.");
            }
            _usersInRaidCount = 0;
            IsLockDownEnabled = false;
            _usersInRaid.Clear();
            _overridenKickMessage = string.Empty;
            _overridenEndTime = DateTime.Now.AddSeconds(-10);
            _raidUsernameBlacklist = string.Empty;
        }
        
        public void EnableLockdown(int duration = -1, string kickMessage = "", string username = "")
        {
            if (duration > _raidSettings.MaxManualLockDownDuration)
                duration = _raidSettings.MaxManualLockDownDuration;
            else if (duration <= 0)
                duration = _raidSettings.DefaultLockDownDuration;
            
            if (kickMessage != string.Empty)
                _overridenKickMessage = kickMessage;
            StartLockdown(username: username, duration: duration, kickMessage: kickMessage);
        }

        private void StartLockdown(string username = "", int duration = 300, string kickMessage = "")
        {
            IsLockDownEnabled = true;
            _raidStartTime = DateTime.Now;
            _overridenEndTime = DateTime.Now.AddSeconds(duration);
            _raidUsernameBlacklist = username.ToLower();
        }
    }
}