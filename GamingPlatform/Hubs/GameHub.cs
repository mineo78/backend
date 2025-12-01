using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace GamingPlatform.Hubs
{
    public class GameHub : Hub
    {
        private static int[][] _board = new int[6][] 
        {
            new int[7], new int[7], new int[7], new int[7], new int[7], new int[7]
        };

        private static int _currentPlayer = 1;

        private string currentPlayer = "Player1"; // Le joueur qui doit jouer en premier
private string player1 = "Player1";
private string player2 = "Player2";

        public GameHub()
        {
            if (_board[0][0] == 0) // Initialise une seule fois le tableau
            {
                for (int i = 0; i < 6; i++)
                    _board[i] = new int[7];
            }
        }

        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("InitBoard", _board);
            return base.OnConnectedAsync();
        }

        public async Task MakeMove(int row, int col)
        {
            // Vérifie si le mouvement est valide
            if (row < 0 || row >= 6 || col < 0 || col >= 7 || _board[row][col] != 0)
                return;

            // Place le jeton du joueur actuel
            _board[row][col] = _currentPlayer;

            // Vérifie si le joueur actuel a gagné
            if (CheckWin(_currentPlayer))
            {
                await Clients.All.SendAsync("GameOver", _currentPlayer);
                ResetBoard();
                return;
            }

            // Passe au joueur suivant
            _currentPlayer = _currentPlayer == 1 ? 2 : 1;

            // Notifie tous les clients de la mise à jour du tableau
            await Clients.All.SendAsync("UpdateBoard", _board);
        }

        private bool CheckWin(int player)
        {
            // Vérifie les alignements horizontaux
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col <= 7 - 4; col++)
                {
                    if (_board[row][col] == player &&
                        _board[row][col + 1] == player &&
                        _board[row][col + 2] == player &&
                        _board[row][col + 3] == player)
                    {
                        return true;
                    }
                }
            }

            // Vérifie les alignements verticaux
            for (int col = 0; col < 7; col++)
            {
                for (int row = 0; row <= 6 - 4; row++)
                {
                    if (_board[row][col] == player &&
                        _board[row + 1][col] == player &&
                        _board[row + 2][col] == player &&
                        _board[row + 3][col] == player)
                    {
                        return true;
                    }
                }
            }

            // Vérifie les alignements diagonaux (haut-gauche vers bas-droite)
            for (int row = 0; row <= 6 - 4; row++)
            {
                for (int col = 0; col <= 7 - 4; col++)
                {
                    if (_board[row][col] == player &&
                        _board[row + 1][col + 1] == player &&
                        _board[row + 2][col + 2] == player &&
                        _board[row + 3][col + 3] == player)
                    {
                        return true;
                    }
                }
            }

            // Vérifie les alignements diagonaux (haut-droite vers bas-gauche)
            for (int row = 0; row <= 6 - 4; row++)
            {
                for (int col = 3; col < 7; col++)
                {
                    if (_board[row][col] == player &&
                        _board[row + 1][col - 1] == player &&
                        _board[row + 2][col - 2] == player &&
                        _board[row + 3][col - 3] == player)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void ResetBoard()
        {
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 7; j++)
                    _board[i][j] = 0;

            _currentPlayer = 1;
        }
    }
}
