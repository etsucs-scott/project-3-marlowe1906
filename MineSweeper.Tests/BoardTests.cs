using MineSweeper.Core;
using Xunit;

namespace MineSweeper.Tests;

// Tests the basic game rules.
public sealed class BoardTests
{
    // Verifies the 8x8 board has 10 mines.
    [Fact]
    public void SmallBoard_HasTenMines()
    {
        MinesweeperGame game = new(BoardOption.All[0], 12345);

        Assert.Equal(10, CountMines(game));
    }

    // Verifies the 12x12 board has 25 mines.
    [Fact]
    public void MediumBoard_HasTwentyFiveMines()
    {
        MinesweeperGame game = new(BoardOption.All[1], 12345);

        Assert.Equal(25, CountMines(game));
    }

    // Verifies the 16x16 board has 40 mines.
    [Fact]
    public void LargeBoard_HasFortyMines()
    {
        MinesweeperGame game = new(BoardOption.All[2], 12345);

        Assert.Equal(40, CountMines(game));
    }

    // Verifies the same seed makes the same mine layout.
    [Fact]
    public void SameSeed_MakesSameMineLayout()
    {
        MinesweeperGame first = new(BoardOption.All[0], 12345);
        MinesweeperGame second = new(BoardOption.All[0], 12345);

        Assert.Equal(GetMineMap(first), GetMineMap(second));
    }

    // Verifies the neighbor counts are correct.
    [Fact]
    public void NeighborCounts_AreCorrect()
    {
        MinesweeperGame game = new(BoardOption.All[0], 12345);

        for (int row = 0; row < game.Option.Size; row++)
        {
            for (int column = 0; column < game.Option.Size; column++)
            {
                Cell cell = game.Cells[row, column];

                if (cell.HasMine)
                {
                    continue;
                }

                Assert.Equal(CountNeighborMines(game, row, column), cell.NeighborMines);
            }
        }
    }

    // Verifies flagging and unflagging works.
    [Fact]
    public void ToggleFlag_FlagsAndUnflags()
    {
        MinesweeperGame game = new(BoardOption.All[0], 12345);

        string firstMessage = game.ToggleFlag(0, 0);
        string secondMessage = game.ToggleFlag(0, 0);

        Assert.Equal("Flag placed.", firstMessage);
        Assert.Equal("Flag removed.", secondMessage);
        Assert.False(game.Cells[0, 0].IsFlagged);
    }

    // Verifies flagged cells cannot be revealed.
    [Fact]
    public void FlaggedCell_CannotBeRevealed()
    {
        MinesweeperGame game = new(BoardOption.All[0], 12345);
        game.ToggleFlag(0, 0);

        string message = game.RevealTile(0, 0);

        Assert.Equal("Flagged tiles cannot be revealed until they are unflagged.", message);
        Assert.False(game.Cells[0, 0].IsRevealed);
    }

    // Verifies revealing a zero cell cascades.
    [Fact]
    public void ZeroCell_RevealsNeighbors()
    {
        (MinesweeperGame game, int row, int column) = CreateGameWithZeroCell();

        game.RevealTile(row, column);

        Assert.True(game.Cells[row, column].IsRevealed);
        Assert.True(CountRevealedSafeCells(game) > 1);
    }

    // Verifies revealing a mine loses the game.
    [Fact]
    public void Mine_RevealLosesGame()
    {
        MinesweeperGame game = new(BoardOption.All[0], 12345);
        (int row, int column) = FindMine(game);

        string message = game.RevealTile(row, column);

        Assert.Equal("Boom! You hit a mine.", message);
        Assert.True(game.IsLost);
    }

    // Verifies all mines are shown after a loss.
    [Fact]
    public void LosingGame_RevealsAllMines()
    {
        MinesweeperGame game = new(BoardOption.All[0], 12345);
        (int row, int column) = FindMine(game);
        game.RevealTile(row, column);

        for (int currentRow = 0; currentRow < game.Option.Size; currentRow++)
        {
            for (int currentColumn = 0; currentColumn < game.Option.Size; currentColumn++)
            {
                Cell cell = game.Cells[currentRow, currentColumn];

                if (cell.HasMine)
                {
                    Assert.True(cell.IsRevealed);
                }
            }
        }
    }

    // Verifies revealing every safe cell wins the game.
    [Fact]
    public void RevealingAllSafeCells_WinsGame()
    {
        MinesweeperGame game = new(BoardOption.All[0], 12345);

        for (int row = 0; row < game.Option.Size; row++)
        {
            for (int column = 0; column < game.Option.Size; column++)
            {
                if (!game.Cells[row, column].HasMine)
                {
                    game.RevealTile(row, column);
                }
            }
        }

        Assert.True(game.IsWon);
    }

    // Counts the mines on the board.
    private static int CountMines(MinesweeperGame game)
    {
        int count = 0;

        for (int row = 0; row < game.Option.Size; row++)
        {
            for (int column = 0; column < game.Option.Size; column++)
            {
                if (game.Cells[row, column].HasMine)
                {
                    count++;
                }
            }
        }

        return count;
    }

    // Creates a simple text map of where the mines are.
    private static string GetMineMap(MinesweeperGame game)
    {
        List<char> cells = [];

        for (int row = 0; row < game.Option.Size; row++)
        {
            for (int column = 0; column < game.Option.Size; column++)
            {
                cells.Add(game.Cells[row, column].HasMine ? 'b' : '.');
            }
        }

        return new string([.. cells]);
    }

    // Counts neighboring mines around one cell.
    private static int CountNeighborMines(MinesweeperGame game, int row, int column)
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

                if (game.IsInBounds(nextRow, nextColumn) && game.Cells[nextRow, nextColumn].HasMine)
                {
                    count++;
                }
            }
        }

        return count;
    }

    // Finds a zero-neighbor cell on a predictable seed.
    private static (MinesweeperGame Game, int Row, int Column) CreateGameWithZeroCell()
    {
        foreach (int seed in new[] { 12345, 42, 2026, 8675309 })
        {
            MinesweeperGame game = new(BoardOption.All[0], seed);

            for (int row = 0; row < game.Option.Size; row++)
            {
                for (int column = 0; column < game.Option.Size; column++)
                {
                    Cell cell = game.Cells[row, column];

                    if (!cell.HasMine && cell.NeighborMines == 0)
                    {
                        return (game, row, column);
                    }
                }
            }
        }

        throw new InvalidOperationException("Expected a zero-neighbor cell for the test seeds.");
    }

    // Finds the first mine on the board.
    private static (int Row, int Column) FindMine(MinesweeperGame game)
    {
        for (int row = 0; row < game.Option.Size; row++)
        {
            for (int column = 0; column < game.Option.Size; column++)
            {
                if (game.Cells[row, column].HasMine)
                {
                    return (row, column);
                }
            }
        }

        throw new InvalidOperationException("Expected to find a mine.");
    }

    // Counts revealed safe cells.
    private static int CountRevealedSafeCells(MinesweeperGame game)
    {
        int count = 0;

        for (int row = 0; row < game.Option.Size; row++)
        {
            for (int column = 0; column < game.Option.Size; column++)
            {
                Cell cell = game.Cells[row, column];

                if (!cell.HasMine && cell.IsRevealed)
                {
                    count++;
                }
            }
        }

        return count;
    }
}