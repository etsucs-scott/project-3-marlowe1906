using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace MineSweeper.Core
{
    internal class Menu
    {
        public int Size;
        public int SeedValue;
        public void Create()
        {
            Console.Write("Menu: \n1) 8x8\n2) 10x10\n3) 12x12\nSelect Difficulty: ");
            string Difficulty = Console.ReadLine();

            MapSize map = new MapSize();
            Size = map.Get(Difficulty);

            Console.Write("Seed (blank = current time): ");
            string Seed = Console.ReadLine();

            Seed seed = new Seed();
            SeedValue = seed.Get(Seed);
        }
    }
}
