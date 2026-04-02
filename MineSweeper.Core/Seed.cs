namespace MineSweeper.Core
{
    public class Seed
    {
        public string Get()
        {
            string seed = string.Empty;
            Console.WriteLine("Please enter a seed or click enter to use a random one: ");
            if (string.IsNullOrEmpty(seed))
            {

            }
            return seed;
        }
    }
}
