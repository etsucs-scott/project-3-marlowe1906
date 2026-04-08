using MineSweeper.Core;
using Xunit;

namespace MineSweeper.Tests;

// Tests the high-score file behavior.
public sealed class HighScoreManagerTests : IDisposable
{
    private readonly string testFolder;

    // Creates a temporary folder for file tests.
    public HighScoreManagerTests()
    {
        testFolder = Path.Combine(Path.GetTempPath(), "MineSweeper.Tests", Guid.NewGuid().ToString("N"));
    }

    // Cleans up the temporary folder.
    public void Dispose()
    {
        if (Directory.Exists(testFolder))
        {
            Directory.Delete(testFolder, true);
        }
    }

    // Verifies the file is created when it is missing.
    [Fact]
    public void LoadScores_CreatesMissingFile()
    {
        string filePath = GetFilePath();
        HighScoreManager manager = new(filePath);

        List<HighScoreEntry> scores = manager.LoadScores(out List<string> messages);

        Assert.Empty(scores);
        Assert.True(File.Exists(filePath));
        Assert.Contains(messages, message => message.Contains("created", StringComparison.OrdinalIgnoreCase));
        Assert.Equal(HighScoreEntry.Header, File.ReadLines(filePath).First());
    }

    // Verifies only the top five scores are kept for a board size.
    [Fact]
    public void AddScore_KeepsTopFiveAndUsesMovesForTies()
    {
        string filePath = GetFilePath();
        HighScoreManager manager = new(filePath);
        manager.LoadScores(out _);

        HighScoreEntry[] entries =
        [
            new HighScoreEntry(8, 70, 20, 1, DateTimeOffset.Parse("2026-04-08T10:00:00+00:00")),
            new HighScoreEntry(8, 60, 30, 2, DateTimeOffset.Parse("2026-04-08T10:01:00+00:00")),
            new HighScoreEntry(8, 60, 18, 3, DateTimeOffset.Parse("2026-04-08T10:02:00+00:00")),
            new HighScoreEntry(8, 55, 25, 4, DateTimeOffset.Parse("2026-04-08T10:03:00+00:00")),
            new HighScoreEntry(8, 80, 15, 5, DateTimeOffset.Parse("2026-04-08T10:04:00+00:00")),
            new HighScoreEntry(8, 58, 21, 6, DateTimeOffset.Parse("2026-04-08T10:05:00+00:00")),
            new HighScoreEntry(12, 90, 45, 7, DateTimeOffset.Parse("2026-04-08T10:06:00+00:00"))
        ];

        List<HighScoreEntry> scores = [];

        foreach (HighScoreEntry entry in entries)
        {
            scores = manager.AddScore(entry, out _);
        }

        List<int> seeds = scores.Where(score => score.Size == 8).Select(score => score.Seed).ToList();

        Assert.Equal(5, scores.Count(score => score.Size == 8));
        Assert.Equal([4, 6, 3, 2, 1], seeds);
        Assert.Single(scores.Where(score => score.Size == 12));
    }

    // Verifies malformed rows are ignored instead of crashing.
    [Fact]
    public void LoadScores_IgnoresBadRows()
    {
        string filePath = GetFilePath();
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        File.WriteAllLines(
            filePath,
            [
                HighScoreEntry.Header,
                "8,45,12,99,2026-04-08T10:00:00.0000000+00:00",
                "bad,row",
                "16,not-a-number,20,88,2026-04-08T10:01:00.0000000+00:00"
            ]);

        HighScoreManager manager = new(filePath);
        List<HighScoreEntry> scores = manager.LoadScores(out List<string> messages);

        Assert.Single(scores);
        Assert.Contains(messages, message => message.Contains("Ignored malformed high score row", StringComparison.Ordinal));
    }

    // Builds the test score-file path.
    private string GetFilePath()
    {
        return Path.Combine(testFolder, "data", "highscores.csv");
    }
}