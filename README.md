# Minesweeper

This project is a console-based Minesweeper game written in C# with a simple structure:

- `MineSweeper.Core` contains the game logic and high-score file handling.
- `MineSweeper.Main` contains the console menu, board drawing, and input loop.
- `MineSweeper.Tests` contains deterministic xUnit tests.

## Build and Run

Build the game project:

```bash
dotnet build MineSweeper.Main/MineSweeper.Main.csproj
```

Run the game:

```bash
dotnet run --project MineSweeper.Main/MineSweeper.Main.csproj
```

## Board Sizes

- `8x8` uses `10` mines
- `12x12` uses `25` mines
- `16x16` uses `40` mines

## Input Commands

The game uses `0`-indexed coordinates.

- `r row col` reveals a tile
- `f row col` flags or unflags a tile
- `q` quits the current game and returns to the main menu

Example:

```text
r 2 3
f 1 1
q
```

## Seed Usage

- The game prompts for an integer seed before each round.
- If the seed is left blank, the game creates one from the current time.
- The seed used for the current game is shown on screen.
- The seed fully determines mine placement, so the same board size and seed always create the same mine layout.

## Board Symbols

- `#` hidden tile
- `f` flagged tile
- `b` revealed mine after a loss
- `.` revealed empty tile with `0` adjacent mines
- `1` through `8` revealed number tiles

## High Scores

High scores are stored in:

- `data/highscores.csv`

Rules:

- High score means the fastest completion time in seconds
- Tie-breaker is fewer moves
- Only the top 5 scores are kept for each board size

CSV format:

```csv
size,seconds,moves,seed,timestamp
8,42,18,12345,2026-04-08T12:00:00.0000000-04:00
```

The game safely creates the file if it is missing and ignores malformed rows with a readable notice instead of crashing.

## Run the Unit Tests

```bash
dotnet test MineSweeper.Tests/MineSweeper.Tests.csproj
```

## UML Diagram

The UML class diagram export is included here:

- `docs/minesweeper-uml.png`

## Notes About the Code

- Every class and function includes comments.
- The zero-tile reveal cascade uses a queue so empty areas expand like classic Minesweeper.
