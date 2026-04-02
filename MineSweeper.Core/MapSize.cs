using System;
using System.Collections.Generic;
using System.Text;

namespace MineSweeper.Core
{
    public class MapSize
    {
        // returns an int of how big the square should be 
        public int Get(string Difficulty)
        {
            Difficulty = Difficulty.ToLower();
            if (Difficulty == string.Empty || Difficulty == "3")
            {
                return 12;
            }
            else if (Difficulty == "1")
            {
                return 8;
            }
            else if (Difficulty == "2")
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
