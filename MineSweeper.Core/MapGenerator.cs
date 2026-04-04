using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MineSweeper.Core
{
    public class MapGenerator
    {
        public string Generate(int Seed, int Size)
        {
            Console.Clear();
            string MapData = string.Empty;
            string MapTile = "#";

            int number = Seed;
            List<int> digits = new List<int>();

            Console.WriteLine($"Seed: {Seed}");
            Console.WriteLine();

            while (number > 0)
            {
                digits.Add(number % 10); // Gets 5, then 4, etc.
                number /= 10;            // Becomes 1234, then 123, etc.
            }

            // Top column numbers
            Console.Write("    ");
            for (int i = 1; i <= Size; i++)
            {
                Console.Write(i.ToString().PadLeft(2) + " ");
            }
            Console.WriteLine();

            // Separator
            Console.Write("    ");
            Console.WriteLine(new string('-', Size * 3));

            // Rows
            for (int i = 0; i < Size; i++)
            {
                // Row number, right-aligned
                Console.Write((i + 1).ToString().PadLeft(2) + " |");

                for (int j = 0; j < Size; j++)
                {
                    Console.Write(" # ");
                    MapData += MapTile;
                }
                Console.WriteLine("|");
            }

            // Bottom separator
            Console.Write("    ");
            Console.WriteLine(new string('-', Size * 3));

            return MapData;
        }
    }
}
