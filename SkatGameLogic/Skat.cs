using System;
using System.Collections.Generic;

namespace SkatGameLogic
{
    public class Skat
    {
        public List<Game> Games;
        public List<Player> Players;

        public Skat()
        {
            Players = new List<Player>
            {
                new HumanPlayer("Forehand"),
                new AiPlayer("Middlehand"),
                new AiPlayer("Rearhand")
            };

            Games = new List<Game>();

            Games.Add(new Game(Players));
        }
    }

    public class Game
    {
        public List<Round> Rounds;
        public Rules Rules;
        public List<Player> Players;
        public CardCollection Skat;
        public int BidValue;
        public Player Declarer;

        public Game(List<Player> players)
        {
            Players = players;
            DealCards();
            Declarer = Bidding();
            var lookedAtSkat = Declarer.SwapWithSkat(Skat);
            Rules = Declarer.SelectRules(lookedAtSkat);
            Declarer.Hand.StandardSort(Rules.TrumpSuit);
            Console.WriteLine($"Playing with rules {Rules}");
        }

        private void DealCards()
        {
            var skatDeck = CardCollection.GenerateSkatDeck();
            skatDeck.Shuffle();
            foreach (var player in Players)
            {
                player.Hand = skatDeck.Take(10);
                player.Hand.StandardSort();
            }

            Skat = skatDeck;
        }

        private Player Bidding()
        {
            var firstBidWinner = PlayerBidsOtherPlayer(Players[1], Players[0]);
            return PlayerBidsOtherPlayer(Players[2], firstBidWinner);
        }

        private Player PlayerBidsOtherPlayer(Player bidder, Player listener)
        {
            while (true)
            {
                var bid = bidder.Bid(BidValue);
                if (bid == null)
                {
                    Console.WriteLine(bidder.Name + " passes");
                    return listener;
                }
                BidValue = (int) bid;
                var callsBid = listener.Listen(BidValue);
                if (!callsBid)
                {
                    Console.WriteLine(listener.Name + " passes");
                    return bidder;
                }
            }
        }

        private void PlayRound()
        {
        }
    }

    public struct Rules
    {
        public GameMode GameMode;
        public CardSuit TrumpSuit; // Only needed if color game
        public RuleModifier Modifier;

        public override string ToString() => $"{GameMode}, {TrumpSuit}, {Modifier}";
    }

    public struct Round
    {
        public Player RoundWinner;
    }

    public enum GameMode
    {
        Grand,
        Color,
        Null
    }

    public enum RuleModifier
    {
        Normal,
        Hand,
        Schneider,
        SchneiderAnnounced, // implies Hand
        Schwarz,
        SchwarzAnnounced, // implies Hand
        Open, // implies Hand
        HandOpen // implies Null gamemode
    }
}