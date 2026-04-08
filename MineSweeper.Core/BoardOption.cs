namespace MineSweeper.Core;

// Stores one supported board size and its mine count.
public sealed class BoardOption
{
    // Creates a board option.
    public BoardOption(int size, int mineCount)
    {
        Size = size;
        MineCount = mineCount;
    }

    // Gets the supported board options in menu order.
    public static IReadOnlyList<BoardOption> All { get; } =
    [
        new BoardOption(8, 10),
        new BoardOption(12, 25),
        new BoardOption(16, 40)
    ];

    // Gets the board size.
    public int Size { get; }

    // Gets the mine count.
    public int MineCount { get; }

    // Finds a board option from the main-menu input.
    public static BoardOption? FromMenuChoice(string? input)
    {
        return input switch
        {
            "1" => All[0],
            "2" => All[1],
            "3" => All[2],
            _ => null
        };
    }
}