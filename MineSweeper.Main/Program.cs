using MineSweeper.Core;

Seed seed = new Seed();
MapSize map = new MapSize();

string num = seed.Get();
int sizing = map.Get();

Console.WriteLine(num);
Console.WriteLine(sizing);