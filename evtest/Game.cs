using System.Collections.Generic;
using System.Linq;

namespace evtest
{
    public class Game
    {
        public List<Player> Players { get; set; }
        private List<Pot> Pots { get; set; }

        int p1, p2, p3, p4;
        public Payout Payout { get; set; }

        public Game()
        {
            Payout = Payout.Ten;

            p1 = 1503;
            p2 = 1499;
            p3 = 1499;
            p4 = 1499;

            Players = new List<Player>()
            {
                new Player{Name=$"Player1({p1})"},
                new Player{Name=$"Player2({p2})"},
                new Player{Name=$"Player3({p3})"},
                new Player{Name=$"Player4({p4})"},
            };

            SetupGame();
        }

        public void SetupGame()
        {
            Players[0].Chips = p1;
            Players[1].Chips = p2;
            Players[2].Chips = p3;
            Players[3].Chips = p4;
            Players.ForEach(x => x.EliminatedRound = 0);
        }

        private void SetupHand()
        {
            Pots = new List<Pot>();
        }

        private void SetupPots()
        {
            while (Players.Count(x => x.Chips > 0) > 1)
            {
                var pot = new Pot()
                {
                    Name = Pots.Count < 1 ? "main pot" : $"side pot {Pots.Count}",
                    EligiblePlayers = new List<Player>()
                };

                var lowestChipAmount = Players.Where(x => x.Chips > 0).OrderBy(x => x.Chips).First().Chips;

                foreach (var player in Players.Where(x => x.Chips >= lowestChipAmount))
                {
                    player.Chips -= lowestChipAmount;
                    pot.Chips += lowestChipAmount;
                    pot.EligiblePlayers.Add(player);
                }

                Pots.Add(pot);
            }
        }

        private void ClaimPots()
        {
            while (Pots.Any(x => x.Chips > 0))
            {
                var currentPot = Pots.OrderByDescending(x => x.EligiblePlayers.Count()).First(x => x.Chips > 0);

                currentPot.EligiblePlayers.Shuffle();

                var winningPlayer = currentPot.EligiblePlayers.First();
                winningPlayer.Chips += currentPot.Chips;
                currentPot.Chips = 0;

                foreach (var pot in Pots.Where(x => x.Chips > 0 && x.EligiblePlayers.Contains(winningPlayer)))
                {
                    winningPlayer.Chips += pot.Chips;
                    pot.Chips = 0;
                }
            }
        }

        private void PayTwoPlaces()
        {
            var eliminatedPlayers = Players.Where(x => x.Chips == 0).GroupBy(x => x.EliminatedRound).OrderByDescending(x => x.Key);

            if (eliminatedPlayers.First().Count() > 1)
            {
                var tempList = eliminatedPlayers.First().ToList();
                tempList.Shuffle();
                tempList.First().DollarsWon += GetSecondPlacePay(Payout);
            }
            else
            {
                eliminatedPlayers.First().First().DollarsWon += GetSecondPlacePay(Payout);
            }
        }

        private void PayThreePlaces()
        {
            var eliminatedPlayers = Players.Where(x => x.Chips == 0).GroupBy(x => x.EliminatedRound).OrderByDescending(x => x.Key);

            if (eliminatedPlayers.First().Count() > 1)
            {
                var tempList = eliminatedPlayers.First().ToList();
                tempList.Shuffle();
                tempList[0].DollarsWon += GetSecondPlacePay(Payout);
                tempList[1].DollarsWon += GetThirdPlacePay(Payout);
            }
            else
            {
                eliminatedPlayers.First().First().DollarsWon += GetSecondPlacePay(Payout);

                if (eliminatedPlayers.Skip(1).First().Count() > 1)
                {
                    var tempList = eliminatedPlayers.Skip(1).First().ToList();
                    tempList.First().DollarsWon += GetThirdPlacePay(Payout);
                }
                else
                {
                    eliminatedPlayers.Skip(1).First().First().DollarsWon += GetThirdPlacePay(Payout);
                }
            }
        }

        private void PayFourPlaces()
        {
            var eliminatedPlayers = Players.Where(x => x.Chips == 0).GroupBy(x => x.EliminatedRound).OrderByDescending(x => x.Key);

            if (eliminatedPlayers.First().Count() == 3)
            {
                var tempList = eliminatedPlayers.First().ToList();
                tempList.Shuffle();
                tempList[0].DollarsWon += GetSecondPlacePay(Payout);
                tempList[1].DollarsWon += GetThirdPlacePay(Payout);
                tempList[2].DollarsWon += GetFourthPlacePay(Payout);
            }
            else if (eliminatedPlayers.First().Count() == 2)
            {
                var tempList = eliminatedPlayers.First().ToList();
                tempList.Shuffle();
                tempList[0].DollarsWon += GetSecondPlacePay(Payout);
                tempList[1].DollarsWon += GetThirdPlacePay(Payout);
                eliminatedPlayers.Skip(1).First().First().DollarsWon += GetFourthPlacePay(Payout);
            }
            else if (eliminatedPlayers.First().Count() == 1)
            {
                eliminatedPlayers.First().First().DollarsWon += GetSecondPlacePay(Payout);

                if (eliminatedPlayers.Skip(1).First().Count() == 2)
                {
                    var tempList = eliminatedPlayers.Skip(1).First().ToList();
                    tempList.Shuffle();
                    tempList[0].DollarsWon += GetThirdPlacePay(Payout);
                    tempList[1].DollarsWon += GetFourthPlacePay(Payout);
                }
                else
                {
                    eliminatedPlayers.Skip(1).First().First().DollarsWon += GetThirdPlacePay(Payout);
                    eliminatedPlayers.Skip(2).First().First().DollarsWon += GetFourthPlacePay(Payout);
                }
            }
        }

        private void PayPlayers()
        {
            Players.First(x => x.Chips > 0).DollarsWon += GetFirstPlacePay(Payout);

            switch (Payout)
            {
                case Payout.Five:
                    PayTwoPlaces();
                    break;
                case Payout.Ten:
                case Payout.Hundred:
                    PayThreePlaces();
                    break;
                case Payout.Thousand:
                case Payout.TenThousand:
                    PayFourPlaces();
                    break;
                default:
                    break;
            }
        }

        public void RunGame()
        {

            var hand = 1;

            while (Players.Count(x => x.Chips > 0) > 1)
            {
                SetupHand();

                SetupPots();

                ClaimPots();

                foreach (var player in Players.Where(x => x.Chips == 0 && x.EliminatedRound == 0))
                {
                    player.EliminatedRound = hand;
                }

                hand++;
            }

            PayPlayers();
        }

        private decimal GetFirstPlacePay(Payout payout)
        {
            switch (payout)
            {
                case Payout.Two:
                    return 2m;
                case Payout.Five:
                    return 3.5m;
                case Payout.Ten:
                    return 6m;
                case Payout.Hundred:
                    return 60m;
                case Payout.Thousand:
                    return 600m;
                case Payout.TenThousand:
                    return 6000m;
            }

            return 0m;
        }

        private decimal GetSecondPlacePay(Payout payout)
        {
            switch (payout)
            {
                case Payout.Five:
                    return 1.5m;
                case Payout.Ten:
                    return 2.5m;
                case Payout.Hundred:
                    return 25m;
                case Payout.Thousand:
                    return 200m;
                case Payout.TenThousand:
                    return 2000m;
            }

            return 0m;
        }

        private decimal GetThirdPlacePay(Payout payout)
        {
            switch (payout)
            {
                case Payout.Ten:
                    return 1.5m;
                case Payout.Hundred:
                    return 15m;
                case Payout.Thousand:
                    return 100m;
                case Payout.TenThousand:
                    return 1000m;
            }

            return 0m;
        }

        private decimal GetFourthPlacePay(Payout payout)
        {
            switch (payout)
            {
                case Payout.Thousand:
                    return 100m;
                case Payout.TenThousand:
                    return 1000m;
            }

            return 0m;
        }
    }

    public class Player
    {
        public string Name { get; set; }
        public int Chips { get; set; }
        public decimal DollarsWon { get; set; }
        public int EliminatedRound { get; set; }
    }

    public class Pot
    {
        public int Chips { get; set; }
        public string Name { get; set; }
        public List<Player> EligiblePlayers { get; set; }

        public string CanBeWonBy()
        {
            string winners = "";

            foreach (var player in EligiblePlayers)
            {
                winners += player.Name + " ";
            }

            return winners;
        }
    }
}
