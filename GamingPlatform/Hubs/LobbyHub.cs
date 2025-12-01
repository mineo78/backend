using GamePlateforme.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamingPlatform.Hubs
{
    public class LobbyHub : Hub
    {
        private static List<Lobby> _lobbies = new(); // Liste des lobbys

        // Récupère tous les lobbys disponibles
        public Task<List<Lobby>> GetLobbies()
        {
            return Task.FromResult(_lobbies);
        }

        // Crée un nouveau lobby
        public async Task CreateLobby(string lobbyName, string playerName)
        {
            var lobby = new Lobby
            {
                Name = lobbyName,
                Players = new List<string> { playerName },
                IsStarted = false
            };
            _lobbies.Add(lobby);

            // Ajouter le créateur au groupe SignalR correspondant au lobby
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyName);

            // Notifie tous les clients que la liste des lobbys a changé
            await Clients.All.SendAsync("UpdateLobbyList", _lobbies);
        }

        // Permet à un joueur de rejoindre un lobby
        public async Task JoinLobby(string lobbyName, string playerName)
        {
            var lobby = _lobbies.FirstOrDefault(l => l.Name == lobbyName);

            if (lobby == null)
            {
                await Clients.Caller.SendAsync("Error", "Lobby introuvable !");
                return;
            }

            if (lobby.Players.Count < 2)
            {
                lobby.Players.Add(playerName);
                await Groups.AddToGroupAsync(Context.ConnectionId, lobbyName);
                await Clients.Group(lobbyName).SendAsync("UpdateLobbyPlayers", lobby.Players);

                if (lobby.Players.Count == 2)
                {
                    lobby.IsStarted = true;
                    await Clients.Group(lobbyName).SendAsync("GameReady", lobby.Name);
                }
            }
            else
            {
                await Clients.Caller.SendAsync("Error", "Le lobby est déjà plein !");
            }
        }
    }
}
