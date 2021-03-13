using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Domain;
using MySql.Data.MySqlClient;

namespace DiscordBot.Services
{
    public class DatabaseService
    {
        private readonly ILoggingService _logging;

        private readonly Settings.Deserialized.Settings _settings;

        private const string API_BASE = "http://localhost:5001";
        private HttpClient _http;

        public DatabaseService(ILoggingService logging, Settings.Deserialized.Settings settings)
        {
            _settings = settings;
            _logging = logging;
            _http = new HttpClient();
        }

        #region User

        public async Task AddNewUser(SocketGuildUser discordUser)
        {
            User user = new User
                        {
                            Username = discordUser.Username,
                            UserId = discordUser.Id,
                            JoinDate = discordUser.JoinedAt?.UtcDateTime ?? DateTime.UtcNow,
                        };

            await _http.PostAsJsonAsync($"{API_BASE}/user/add", user);
        }

        public async Task UpdateUser(User user)
        {
            await _http.PostAsJsonAsync($"{API_BASE}/user/update", user);
        }

        public async Task<User?> GetUser(ulong userId)
        {
            var response = await _http.GetAsync($"{API_BASE}/user/userid/{userId}");
            return await response.Content.ReadAsAsync<User>();
        }

        public async Task<List<User>> GetTopLevel()
        {
            var response = await _http.GetAsync($"{API_BASE}/user/top/level");
            return await response.Content.ReadAsAsync<List<User>>();
        }

        public async Task<List<User>> GetTopKarma()
        {
            var response = await _http.GetAsync($"{API_BASE}/user/top/karma");
            return await response.Content.ReadAsAsync<List<User>>();
        }

        #endregion
    }
}