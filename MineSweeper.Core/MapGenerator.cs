using System;
using System.Collections.Generic;
using System.Text;

namespace MineSweeper.Core
{
    public class MapGenerator
    {
        public void Generate()
        {
            Menu menu = new Menu();

            menu.Create();

            Console.WriteLine(menu.Size);
            Console.WriteLine(menu.SeedValue);
        }
    }
}
