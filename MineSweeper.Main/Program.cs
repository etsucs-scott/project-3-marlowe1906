using MineSweeper.Core;
using System.ComponentModel.Design;

MapGenerator map = new MapGenerator();
Menu menu = new Menu();

menu.Create();

int Seed = menu.SeedValue;
int Size = menu.Size;

string MapData = map.Generate(Seed, Size);

Console.WriteLine(MapData);


