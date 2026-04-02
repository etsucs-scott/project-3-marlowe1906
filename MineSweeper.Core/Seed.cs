namespace MineSweeper.Core
{
    public class Seed
    {
        // returns an int of 6 digits
        public int Get(string seed)
        {
            if (string.IsNullOrEmpty(seed) || seed.Length != 6)
            {
                seed = DateTime.Now.ToString("ffffff");

                //Get rid of leading 0s
                int num = int.Parse(seed);
                seed = num.ToString();
           
                while (seed.Length != 6)
                {
                   seed = "1" + seed;
                }

                num = int.Parse(seed);
                return num;
            }
            else
            {
                return -1;
            }
        }
    }
}
