namespace MineSweeper.Core;

// Stores the full state of one Minesweeper game.
public sealed class MinesweeperGame
{
    private readonly DateTimeOffset startedAt;
    private DateTimeOffset? endedAt;

    // Creates a game for one board option and seed.
    public MinesweeperGame(BoardOption option, int seed)
    {
        Option = option;
        Seed = seed;
        Cells = CreateCells(option.Size);
        startedAt = DateTimeOffset.Now;
        PlaceMines();
        SetNeighborCounts();
    }

    // Gets the selected board option.
    public BoardOption Option { get; }

    // Gets the seed for this game.
    public int Seed { get; }

    // Gets the board cells.
    public Cell[,] Cells { get; }

    // Gets the move count.
    public int MoveCount { get; private set; }

    // Gets a value indicating whether the player won.
    public bool IsWon { get; private set; }

    // Gets a value indicating whether the player lost.
    public bool IsLost { get; private set; }

    // Gets a value indicating whether the game is finished.
    public bool IsFinished => IsWon || IsLost;

    // Gets the elapsed time in whole seconds.
    public int ElapsedSeconds
    {
        get
        {
            DateTimeOffset endTime = endedAt ?? DateTimeOffset.Now;
            return Math.Max(1, (int)Math.Ceiling((endTime - startedAt).TotalSeconds));
        }
    }

    // Checks whether a coordinate is on the board.
    public bool IsInBounds(int row, int column)
    {
        return row >= 0 && row < Option.Size && column >= 0 && column < Option.Size;
    }

    // Reveals one cell and returns a simple status message.
    public string RevealTile(int row, int column)
    {
        Cell cell = Cells[row, column];

        if (IsFinished)
        {
            return "The game is already over.";
        }

        if (cell.IsFlagged)
        {
            return "Flagged tiles cannot be revealed until they are unflagged.";
        }

        if (cell.IsRevealed)
        {
            return "That tile is already revealed.";
        }

        MoveCount++;

        if (cell.HasMine)
        {
            cell.IsRevealed = true;
            RevealAllMines();
            IsLost = true;
            endedAt = DateTimeOffset.Now;
            return "Boom! You hit a mine.";
        }

        RevealArea(row, column);

        if (AllSafeCellsRevealed())
        {
            IsWon = true;
            endedAt = DateTimeOffset.Now;
            return "You revealed the last safe tile.";
        }

        return "Tile revealed.";
    }

    // Flags or unflags one cell and returns a simple status message.
    public string ToggleFlag(int row, int column)
    {
        Cell cell = Cells[row, column];

        if (IsFinished)
        {
            return "The game is already over.";
        }

        if (cell.IsRevealed)
        {
            return "Revealed tiles cannot be flagged.";
        }

        cell.IsFlagged = !cell.IsFlagged;
        MoveCount++;

        return cell.IsFlagged ? "Flag placed." : "Flag removed.";
    }

    // Creates a score entry for a win.
    public HighScoreEntry CreateScoreEntry()
    {
        return new HighScoreEntry(Option.Size, ElapsedSeconds, MoveCount, Seed, DateTimeOffset.Now);
    }

    // Creates the 2D cell array.
    private static Cell[,] CreateCells(int size)
    {
        Cell[,] cells = new Cell[size, size];

        for (int row = 0; row < size; row++)
        {
            for (int column = 0; column < size; column++)
            {
                cells[row, column] = new Cell();
            }
        }

        return cells;
    }

    // Places mines using the seed.
    private void PlaceMines()
    {
        List<int> positions = Enumerable.Range(0, Option.Size * Option.Size).ToList();
        Random random = new(Seed);

        for (int index = positions.Count - 1; index > 0; index--)
        {
            int swapIndex = random.Next(index + 1);
            (positions[index], positions[swapIndex]) = (positions[swapIndex], positions[index]);
        }

        for (int index = 0; index < Option.MineCount; index++)
        {
            int position = positions[index];
            int row = position / Option.Size;
            int column = position % Option.Size;
            Cells[row, column].HasMine = true;
        }
    }

    // Sets the adjacent mine counts for each safe cell.
    private void SetNeighborCounts()
    {
        for (int row = 0; row < Option.Size; row++)
        {
            for (int column = 0; column < Option.Size; column++)
            {
                if (Cells[row, column].HasMine)
                {
                    continue;
                }

                Cells[row, column].NeighborMines = CountNeighborMines(row, column);
            }
        }
    }

    // Counts neighboring mines around one cell.
    private int CountNeighborMines(int row, int column)
    {
        int count = 0;

        for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
        {
            for (int columnOffset = -1; columnOffset <= 1; columnOffset++)
            {
                if (rowOffset == 0 && columnOffset == 0)
                {
                    continue;
                }

                int nextRow = row + rowOffset;
                int nextColumn = column + columnOffset;

                if (IsInBounds(nextRow, nextColumn) && Cells[nextRow, nextColumn].HasMine)
                {
                    count++;
                }
            }
        }

        return count;
    }

    // Reveals a cell and cascades through empty neighbors.
    private void RevealArea(int startRow, int startColumn)
    {
        Queue<(int Row, int Column)> cellsToReveal = new();
        cellsToReveal.Enqueue((startRow, startColumn));

        // This queue handles the classic zero-tile cascade in a straightforward way.
        while (cellsToReveal.Count > 0)
        {
            (int row, int column) = cellsToReveal.Dequeue();
            Cell cell = Cells[row, column];

            if (cell.IsRevealed || cell.IsFlagged)
            {
                continue;
            }

            cell.IsRevealed = true;

            if (cell.NeighborMines != 0)
            {
                continue;
            }

            for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
            {
                for (int columnOffset = -1; columnOffset <= 1; columnOffset++)
                {
                    if (rowOffset == 0 && columnOffset == 0)
                    {
                        continue;
                    }

                    int nextRow = row + rowOffset;
                    int nextColumn = column + columnOffset;

                    if (IsInBounds(nextRow, nextColumn)
                        && !Cells[nextRow, nextColumn].HasMine
                        && !Cells[nextRow, nextColumn].IsRevealed)
                    {
                        cellsToReveal.Enqueue((nextRow, nextColumn));
                    }
                }
            }
        }
    }

    // Reveals all mines after a loss.
    private void RevealAllMines()
    {
        for (int row = 0; row < Option.Size; row++)
        {
            for (int column = 0; column < Option.Size; column++)
            {
                if (Cells[row, column].HasMine)
                {
                    Cells[row, column].IsRevealed = true;
                }
            }
        }
    }

    // Checks whether every safe cell has been revealed.
    private bool AllSafeCellsRevealed()
    {
        for (int row = 0; row < Option.Size; row++)
        {
            for (int column = 0; column < Option.Size; column++)
            {
                Cell cell = Cells[row, column];

                if (!cell.HasMine && !cell.IsRevealed)
                {
                    return false;
                }
            }
        }

        return true;
    }
}