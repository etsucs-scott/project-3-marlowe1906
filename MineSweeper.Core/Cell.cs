namespace MineSweeper.Core;

// Stores the state of one board cell.
public sealed class Cell
{
    // Gets or sets a value indicating whether this cell contains a mine.
    public bool HasMine { get; set; }

    // Gets or sets a value indicating whether this cell has been revealed.
    public bool IsRevealed { get; set; }

    // Gets or sets a value indicating whether this cell is flagged.
    public bool IsFlagged { get; set; }

    // Gets or sets the number of adjacent mines.
    public int NeighborMines { get; set; }
}