using GamingPlatform.Models;
using GamingPlatform.Services;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace GamingPlatform.Hubs
{
    public class LobbyHub : Hub
    {
        private readonly LobbyService _lobbyService;

        public LobbyHub(LobbyService lobbyService)
        {
            _lobbyService = lobbyService;
        }

        public async Task JoinLobbyGroup(string lobbyId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
        }

        public async Task LeaveLobbyGroup(string lobbyId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId);
        }
        
        public async Task StartGame(string lobbyId)
        {
            if (_lobbyService.StartGame(lobbyId))
            {
                var lobby = _lobbyService.GetLobby(lobbyId);
                await Clients.Group(lobbyId).SendAsync("GameStarted", lobby.GameType);
            }
        }
    }
}
