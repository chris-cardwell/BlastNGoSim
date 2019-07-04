using System;
using System.Collections.Generic;

namespace evtest
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game();
            var gameCount = 1000000;
            var rng = new Random();
            var twoCount = 0;
            var fiveCount = 0;
            var tenCount = 0;
            var hundoCount = 0;
            var thouCount = 0;
            var tenThouCount = 0;

            for (int i = 0; i < gameCount; i++)
            {
                var number = rng.Next(1000000);

                if (number < 565500)
                {
                    game.Payout = Payout.Two;
                    twoCount++;
                }
                else if (number < 565500 + 402300)
                {
                    game.Payout = Payout.Five;
                    fiveCount++;
                }
                else if (number < 565500 + 402300 + 31750)
                {
                    game.Payout = Payout.Ten;
                    tenCount++;
                }
                else if (number < 565500 + 402300 + 31750 + 400)
                {
                    game.Payout = Payout.Hundred;
                    hundoCount++;
                }
                else if (number < 565500 + 402300 + 31750 + 400 + 40)
                {
                    game.Payout = Payout.Thousand;
                    thouCount++;
                }
                else
                {
                    game.Payout = Payout.TenThousand;
                    tenThouCount++;
                }

                game.RunGame();
                game.SetupGame();
            }

            Console.WriteLine($"Two: {twoCount} | Five: {fiveCount} | Ten: {tenCount} | Hun: {hundoCount} | Thou: {thouCount} | TenThou: {tenThouCount}");

            var totalPaid = (twoCount * 2) + (fiveCount * 5) + (tenCount * 10) + (hundoCount * 100) + (thouCount * 1000) + (tenThouCount * 10000);

            foreach (var player in game.Players)
            {
                Console.WriteLine($"{player.Name}: net: {(-gameCount + player.DollarsWon).ToString("C")} | {(player.DollarsWon / totalPaid * 100).ToString("0.00")}%");
            }
        }

    }

    public static class ShuffleExtension
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

    public enum Payout
    {
        Two = 2,
        Five = 5,
        Ten = 10,
        Hundred = 100,
        Thousand = 1000,
        TenThousand = 10000
    }
}
