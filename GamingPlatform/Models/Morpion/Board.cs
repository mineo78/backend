public class Board
{
    private int totalPiecesPlaced;

    public Board()
    {
        this.Pieces = new string[3][];
        for (int i = 0; i < 3; i++)
        {
            this.Pieces[i] = new string[3];
        }
    }

    /// <summary>
    /// Represents the pieces on the board.
    /// </summary>
    public string[][] Pieces { get; private set; }

    public bool IsThreeInRow
    {
        get
        {
            // Check all rows
            for (int row = 0; row < this.Pieces.Length; row++)
            {
                if (!string.IsNullOrWhiteSpace(Pieces[row][0]) &&
                    Pieces[row][0] == Pieces[row][1] &&
                    Pieces[row][1] == Pieces[row][2])
                {
                    return true;
                }
            }

            // Check all columns
            for (int col = 0; col < this.Pieces[0].Length; col++)
            {
                if (!string.IsNullOrWhiteSpace(Pieces[0][col]) &&
                    Pieces[0][col] == Pieces[1][col] &&
                    Pieces[1][col] == Pieces[2][col])
                {
                    return true;
                }
            }

            // Check diagonals
            if (!string.IsNullOrWhiteSpace(Pieces[1][1]) &&
                ((Pieces[0][0] == Pieces[1][1] && Pieces[1][1] == Pieces[2][2]) ||
                (Pieces[0][2] == Pieces[1][1] && Pieces[1][1] == Pieces[2][0])))
            {
                return true;
            }

            return false;
        }
    }

    public bool AreSpacesLeft => this.totalPiecesPlaced < 9;

    public void PlacePiece(int row, int col, string pieceToPlace)
    {
        this.Pieces[row][col] = pieceToPlace;
        this.totalPiecesPlaced++;
    }

    public override string ToString()
    {
        return string.Join(", ", this.Pieces.Select(row => string.Join(" ", row)));
    }
}
