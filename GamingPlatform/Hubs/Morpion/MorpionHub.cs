using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using GamingPlatform.Models;

namespace GamingPlatform.Hubs.Morpion
{
    public class MorpionHub : Hub
    {
        private readonly GameState _gameState;


        public MorpionHub(GameState gameState)
        {
            _gameState = gameState;
        }

       

        public async Task FindGame(string username)
        {
            Console.WriteLine($"Youpooooooooooo {username}");
            if (_gameState.IsUsernameTaken(username))
            {
                await Clients.Caller.SendAsync("usernameTaken");
                return;
            }

            var joiningPlayer = _gameState.CreatePlayer(username, Context.ConnectionId);
            await Clients.Caller.SendAsync("playerJoined", joiningPlayer);

            var opponent = _gameState.GetWaitingOpponent();
            if (opponent == null)
            {
                _gameState.AddToWaitingPool(joiningPlayer);
                await Clients.Caller.SendAsync("waitingList");
            }
            else
            {
                var newGame = await _gameState.CreateGame(opponent, joiningPlayer);
                await Clients.Group(newGame.Id).SendAsync("start", newGame);
            }
        }

        public async Task PlacePiece(int row, int col)
        {
            var playerMakingTurn = _gameState.GetPlayer(Context.ConnectionId);
            if (playerMakingTurn == null)
            {
                await Clients.Caller.SendAsync("notPlayersTurn");
                return;
            }

            if (!_gameState.GetGame(playerMakingTurn, out var opponent)?.WhoseTurn.Equals(playerMakingTurn) ?? true)
            {
                await Clients.Caller.SendAsync("notPlayersTurn");
                return;
            }

            if (!_gameState.GetGame(playerMakingTurn, out opponent)?.IsValidMove(row, col) ?? false)
            {
                await Clients.Caller.SendAsync("notValidMove");
                return;
            }

            var game = _gameState.GetGame(playerMakingTurn, out opponent);
            game.PlacePiece(row, col);

            await Clients.Group(game.Id).SendAsync("piecePlaced", row, col, playerMakingTurn.Piece);

            if (!game.IsOver)
            {
                await Clients.Group(game.Id).SendAsync("updateTurn", game);
            }
            else
            {
                if (game.IsTie)
                {
                    await Clients.Group(game.Id).SendAsync("tieGame");
                }
                else
                {
                    await Clients.Group(game.Id).SendAsync("winner", playerMakingTurn.Name);
                }

                _gameState.RemoveGame(game.Id);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var leavingPlayer = _gameState.GetPlayer(Context.ConnectionId);

            if (leavingPlayer != null)
            {
                Console.WriteLine($"Player disconnected: {leavingPlayer.Id}");

       
                if (_gameState.GetGame(leavingPlayer, out var opponent) is { } ongoingGame)
                {
                    Console.WriteLine($"Notifying opponent of player {leavingPlayer.Id} leaving.");
                    await Clients.Group(ongoingGame.Id).SendAsync("opponentLeft");

                    _gameState.RemoveGame(ongoingGame.Id);
                }
                else
                {
                    Console.WriteLine($"Player {leavingPlayer.Id} is not in any game.");
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

    }
}
