namespace MineSweeper.Core;

// Handles loading and saving high scores in the required CSV file.
public sealed class HighScoreManager
{
    // Creates a manager for one CSV file path.
    public HighScoreManager(string filePath)
    {
        FilePath = filePath;
    }

    // Gets the score file path.
    public string FilePath { get; }

    // Loads all valid scores from the file.
    public List<HighScoreEntry> LoadScores(out List<string> messages)
    {
        messages = [];
        List<HighScoreEntry> scores = [];

        try
        {
            EnsureFileExists(messages);
            string[] lines = File.ReadAllLines(FilePath);

            if (lines.Length == 0 || !string.Equals(lines[0], HighScoreEntry.Header, StringComparison.OrdinalIgnoreCase))
            {
                messages.Add("The high score file header was invalid, so the file was reset.");
                SaveScores(scores, messages);
                return scores;
            }

            for (int index = 1; index < lines.Length; index++)
            {
                string line = lines[index];

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                if (HighScoreEntry.TryParse(line, out HighScoreEntry? entry) && entry is not null)
                {
                    scores.Add(entry);
                }
                else
                {
                    messages.Add($"Ignored malformed high score row {index + 1}.");
                }
            }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            messages.Add($"Unable to load high scores: {exception.Message}");
        }

        return KeepTopFive(scores);
    }

    // Adds one score, keeps the top five per board size, and saves the file.
    public List<HighScoreEntry> AddScore(HighScoreEntry entry, out List<string> messages)
    {
        List<HighScoreEntry> scores = LoadScores(out messages);

        if (messages.Any(message => message.StartsWith("Unable to load", StringComparison.Ordinal)))
        {
            return scores;
        }

        scores.Add(entry);
        scores = KeepTopFive(scores);
        SaveScores(scores, messages);
        return scores;
    }

    // Makes sure the score file exists.
    private void EnsureFileExists(List<string> messages)
    {
        string? folder = Path.GetDirectoryName(FilePath);

        if (!string.IsNullOrWhiteSpace(folder) && !Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        if (!File.Exists(FilePath))
        {
            File.WriteAllText(FilePath, HighScoreEntry.Header + Environment.NewLine);
            messages.Add("The high score file was missing and has been created.");
        }
    }

    // Saves scores to the CSV file.
    private void SaveScores(List<HighScoreEntry> scores, List<string> messages)
    {
        try
        {
            List<string> lines = [HighScoreEntry.Header];
            lines.AddRange(scores.Select(score => score.ToCsvLine()));
            File.WriteAllLines(FilePath, lines);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            messages.Add($"Unable to save high scores: {exception.Message}");
        }
    }

    // Keeps only the top five scores for each board size.
    private static List<HighScoreEntry> KeepTopFive(List<HighScoreEntry> scores)
    {
        return scores
            .GroupBy(score => score.Size)
            .SelectMany(group => group
                .OrderBy(score => score.Seconds)
                .ThenBy(score => score.Moves)
                .ThenBy(score => score.Timestamp)
                .Take(5))
            .OrderBy(score => score.Size)
            .ThenBy(score => score.Seconds)
            .ThenBy(score => score.Moves)
            .ThenBy(score => score.Timestamp)
            .ToList();
    }
}