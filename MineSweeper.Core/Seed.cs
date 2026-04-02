namespace MineSweeper.Core
{
    public class Seed
    {
        public string Get()
        {
            string seed = string.Empty;
            Console.WriteLine("Please enter a numarical 6 digit seed or click enter to use a random one: ");
            seed = Console.ReadLine();
            if (string.IsNullOrEmpty(seed) || seed.Length != 6)
            {
                DateTime time = DateTime.Now;
                seed = DateTime.Now.ToString("ffffff");
            }
            return seed;
        }
    }
}
