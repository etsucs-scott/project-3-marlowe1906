using MineSweeper.Core;

namespace MineSweeper.Main;

// Runs the console version of Minesweeper.
public static class Program
{
    // Starts the application.
    public static void Main()
    {
        string highScorePath = Path.Combine(GetDataFolder(), "highscores.csv");
        HighScoreManager highScoreManager = new(highScorePath);
        Run(highScoreManager);
    }

    // Runs the main menu loop.
    private static void Run(HighScoreManager highScoreManager)
    {
        highScoreManager.LoadScores(out List<string> startupMessages);
        ShowMessages(startupMessages, true);

        while (true)
        {
            BoardOption? option = PromptBoardOption();

            if (option is null)
            {
                return;
            }

            int seed = PromptSeed();
            MinesweeperGame game = new(option, seed);
            PlayGame(game, highScoreManager);
        }
    }

    // Prompts the player to choose a board size.
    private static BoardOption? PromptBoardOption()
    {
        while (true)
        {
            ClearScreen();
            Console.WriteLine("Menu:");
            Console.WriteLine("1) 8x8");
            Console.WriteLine("2) 12x12");
            Console.WriteLine("3) 16x16");
            Console.Write("Choose a board size (or q to quit): ");

            string? rawInput = Console.ReadLine();

            if (rawInput is null)
            {
                return null;
            }

            string input = rawInput.Trim();

            if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            BoardOption? option = BoardOption.FromMenuChoice(input);

            if (option is not null)
            {
                return option;
            }

            PauseWithMessage("Invalid menu choice. Enter 1, 2, 3, or q.");
        }
    }

    // Prompts the player for a seed.
    private static int PromptSeed()
    {
        while (true)
        {
            Console.Write("Seed (blank = time): ");
            string? rawInput = Console.ReadLine();

            if (rawInput is null)
            {
                return unchecked((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            }

            string input = rawInput.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                return unchecked((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            }

            if (int.TryParse(input, out int seed))
            {
                return seed;
            }

            PauseWithMessage("Seed must be a valid integer.");
        }
    }

    // Runs one game until the player wins, loses, or quits to the menu.
    private static void PlayGame(MinesweeperGame game, HighScoreManager highScoreManager)
    {
        string message = string.Empty;

        while (!game.IsFinished)
        {
            ClearScreen();
            DrawBoard(game);

            if (!string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine();
                Console.WriteLine(message);
            }

            Console.WriteLine();
            Console.Write("> ");
            string? input = Console.ReadLine();

            if (input is null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                message = "Invalid command. Use r row col, f row col, or q.";
                continue;
            }

            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (parts.Length == 1 && string.Equals(parts[0], "q", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (parts.Length != 3)
            {
                message = "Invalid command. Use r row col, f row col, or q.";
                continue;
            }

            if (!int.TryParse(parts[1], out int row) || !int.TryParse(parts[2], out int column))
            {
                message = "Row and column must be integers.";
                continue;
            }

            if (!game.IsInBounds(row, column))
            {
                message = $"Coordinates must be between 0 and {game.Option.Size - 1}.";
                continue;
            }

            if (parts[0].Equals("r", StringComparison.OrdinalIgnoreCase))
            {
                message = game.RevealTile(row, column);
            }
            else if (parts[0].Equals("f", StringComparison.OrdinalIgnoreCase))
            {
                message = game.ToggleFlag(row, column);
            }
            else
            {
                message = "Unknown command. Use r row col, f row col, or q.";
            }
        }

        FinishGame(game, highScoreManager);
    }

    // Draws the current board and game information.
    private static void DrawBoard(MinesweeperGame game)
    {
        Console.WriteLine($"Board: {game.Option.Size}x{game.Option.Size} | Mines: {game.Option.MineCount}");
        Console.WriteLine($"Seed: {game.Seed}");
        Console.WriteLine($"Moves: {game.MoveCount}");
        Console.WriteLine("Commands: r row col | f row col | q");
        Console.WriteLine();

        Console.Write("   ");
        for (int column = 0; column < game.Option.Size; column++)
        {
            Console.Write($"{column,2} ");
        }

        Console.WriteLine();

        for (int row = 0; row < game.Option.Size; row++)
        {
            Console.Write($"{row,2} ");

            for (int column = 0; column < game.Option.Size; column++)
            {
                Console.Write($"{GetCellSymbol(game.Cells[row, column]),2} ");
            }

            Console.WriteLine();
        }
    }

    // Converts one cell into the required display symbol.
    private static string GetCellSymbol(Cell cell)
    {
        if (cell.IsRevealed)
        {
            if (cell.HasMine)
            {
                return "b";
            }

            return cell.NeighborMines == 0 ? "." : cell.NeighborMines.ToString();
        }

        return cell.IsFlagged ? "f" : "#";
    }

    // Shows the end-of-game screen and updates scores after a win.
    private static void FinishGame(MinesweeperGame game, HighScoreManager highScoreManager)
    {
        ClearScreen();
        DrawBoard(game);
        Console.WriteLine();

        if (game.IsWon)
        {
            Console.WriteLine($"You win in {game.ElapsedSeconds} seconds using {game.MoveCount} moves.");
            HighScoreEntry newScore = game.CreateScoreEntry();
            List<HighScoreEntry> scores = highScoreManager.AddScore(newScore, out List<string> messages);
            ShowMessages(messages, false);
            ShowLeaderboard(scores, game.Option.Size, newScore);
        }
        else
        {
            Console.WriteLine("You hit a mine. Better luck next time.");
        }

        Console.WriteLine();
        Console.Write("Press Enter to return to the main menu...");
        Console.ReadLine();
    }

    // Shows the top five scores for one board size.
    private static void ShowLeaderboard(List<HighScoreEntry> scores, int size, HighScoreEntry newScore)
    {
        List<HighScoreEntry> boardScores = scores.Where(score => score.Size == size).Take(5).ToList();
        Console.WriteLine($"Top 5 High Scores for {size}x{size}:");

        if (boardScores.Count == 0)
        {
            Console.WriteLine("No scores recorded yet.");
            return;
        }

        bool madeLeaderboard = boardScores.Any(score =>
            score.Seconds == newScore.Seconds
            && score.Moves == newScore.Moves
            && score.Seed == newScore.Seed
            && score.Timestamp == newScore.Timestamp);

        Console.WriteLine(madeLeaderboard ? "Your score made the leaderboard." : "Your score did not make the leaderboard.");

        for (int index = 0; index < boardScores.Count; index++)
        {
            HighScoreEntry score = boardScores[index];
            Console.WriteLine($"{index + 1}. {score.Seconds}s | {score.Moves} moves | seed {score.Seed} | {score.Timestamp:yyyy-MM-dd HH:mm:ss}");
        }
    }

    // Displays file messages.
    private static void ShowMessages(List<string> messages, bool pauseAfter)
    {
        List<string> cleanMessages = messages
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .Distinct()
            .ToList();

        if (cleanMessages.Count == 0)
        {
            return;
        }

        Console.WriteLine();

        foreach (string message in cleanMessages)
        {
            Console.WriteLine($"Notice: {message}");
        }

        Console.WriteLine();

        if (pauseAfter)
        {
            Console.Write("Press Enter to continue...");
            Console.ReadLine();
        }
    }

    // Shows a message and waits for Enter.
    private static void PauseWithMessage(string message)
    {
        Console.WriteLine();
        Console.WriteLine(message);
        Console.Write("Press Enter to continue...");
        Console.ReadLine();
    }

    // Finds the repository data folder.
    private static string GetDataFolder()
    {
        string[] startingPoints =
        [
            Directory.GetCurrentDirectory(),
            AppContext.BaseDirectory
        ];

        foreach (string start in startingPoints)
        {
            string? current = start;

            // This keeps the save file under the repo's data folder even when dotnet run starts from bin.
            while (!string.IsNullOrWhiteSpace(current))
            {
                if (File.Exists(Path.Combine(current, "MineSweeper.sln")) || File.Exists(Path.Combine(current, "MineSweeper.slnx")))
                {
                    string dataFolder = Path.Combine(current, "data");
                    Directory.CreateDirectory(dataFolder);
                    return dataFolder;
                }

                current = Directory.GetParent(current)?.FullName;
            }
        }

        string fallback = Path.Combine(Directory.GetCurrentDirectory(), "data");
        Directory.CreateDirectory(fallback);
        return fallback;
    }

    // Clears the console when possible and safely ignores redirected-output cases.
    private static void ClearScreen()
    {
        try
        {
            Console.Clear();
        }
        catch (IOException)
        {
        }
    }
}