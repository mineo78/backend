namespace GamePlateforme.Models
{
    public class GameBoard
    {
        public int[][] Board { get; set; }
        public int CurrentPlayer { get; set; }

        public GameBoard()
        {
            Board = new int[6][];
            for (int i = 0; i < 6; i++)
                Board[i] = new int[7];

            CurrentPlayer = 1;
        }
    }
}
