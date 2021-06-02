namespace DiscordBot.Settings.Deserialized
{
    public class RaidProtection
    {
        // Attached to any public facing messages.
        public string RaidProtectionIdentifier { get; set; } = "**Raid Protect**:";
        
        public string KickMessage { get; set; } = "The server is currently in lockdown, likely due to bots.\nYou have been kicked while this period is active. Please try again in 5-10 minutes.";
        
        // Only activates when `MaxNewUsers` joins within `MaxJoinSeconds`
        public int MaxNewUsers { get; set; } = 4;
        public float MaxJoinSeconds { get; set; } = 2;

        // How long a manual lockdown can last in seconds.
        public int MaxManualLockDownDuration { get; set; } = 600;

    }
}