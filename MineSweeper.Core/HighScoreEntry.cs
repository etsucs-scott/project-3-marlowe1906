using System.Globalization;

namespace MineSweeper.Core;

// Stores one high-score row.
public sealed class HighScoreEntry
{
    // Creates a high-score entry.
    public HighScoreEntry(int size, int seconds, int moves, int seed, DateTimeOffset timestamp)
    {
        Size = size;
        Seconds = seconds;
        Moves = moves;
        Seed = seed;
        Timestamp = timestamp;
    }

    // Gets the required CSV header.
    public const string Header = "size,seconds,moves,seed,timestamp";

    // Gets the board size.
    public int Size { get; }

    // Gets the completion time in seconds.
    public int Seconds { get; }

    // Gets the move count.
    public int Moves { get; }

    // Gets the seed.
    public int Seed { get; }

    // Gets the timestamp.
    public DateTimeOffset Timestamp { get; }

    // Converts the entry to one CSV line.
    public string ToCsvLine()
    {
        return string.Join(
            ',',
            Size.ToString(CultureInfo.InvariantCulture),
            Seconds.ToString(CultureInfo.InvariantCulture),
            Moves.ToString(CultureInfo.InvariantCulture),
            Seed.ToString(CultureInfo.InvariantCulture),
            Timestamp.ToString("O", CultureInfo.InvariantCulture));
    }

    // Tries to parse one CSV line.
    public static bool TryParse(string line, out HighScoreEntry? entry)
    {
        entry = null;
        string[] parts = line.Split(',', StringSplitOptions.TrimEntries);

        if (parts.Length != 5)
        {
            return false;
        }

        int size = 0;
        int seconds = 0;
        int moves = 0;
        int seed = 0;
        DateTimeOffset timestamp = default;

        bool parsed = int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out size)
            && int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out seconds)
            && int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out moves)
            && int.TryParse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out seed)
            && DateTimeOffset.TryParse(parts[4], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out timestamp);

        if (!parsed)
        {
            return false;
        }

        if ((size != 8 && size != 12 && size != 16) || seconds < 0 || moves < 0)
        {
            return false;
        }

        entry = new HighScoreEntry(size, seconds, moves, seed, timestamp);
        return true;
    }
}