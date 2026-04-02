using System;
using System.Collections.Generic;
using System.Text;

namespace MineSweeper.Core
{
    public class MapSize
    {
        public int Get()
        {
            Console.WriteLine("What difficulty do you want: easy, medium, or hard? (nothing = hard): ");
            string Difficulty = Console.ReadLine();
            Difficulty = Difficulty.ToLower();
            if (Difficulty == string.Empty || Difficulty == "hard")
            {
                return 12;
            }
            else if (Difficulty == "easy")
            {
                return 8;
            }
            else if (Difficulty == "medium")
            {
                return 10;
            }
            else
            {
                Console.WriteLine("Uh oh! Something went wrong! Hope you wanted it hard!");
                return 12;
            }
        }
    }
}
