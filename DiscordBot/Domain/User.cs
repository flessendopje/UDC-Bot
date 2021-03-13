using System;

namespace DiscordBot.Domain
{
    public class User
    {
        public string? Id { get; set; }
        public DateTime CreatedAt { get; }
        public string Username { get; set; }
        public ulong UserId { get; set; }
        public DateTime JoinDate { get; set; }
        public int Karma { get; set; }
        public int KarmaGiven { get; set; }
        public uint Experience { get; set; }
        public uint Level { get; set; }
        public uint Rank { get; set; }
    }
}