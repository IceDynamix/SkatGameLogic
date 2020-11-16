using System.Collections.Generic;

namespace SkatGameLogic
{
    public class Skat
    {
        public List<Game> Games;
        public List<Player> Players;

        public Skat()
        {
            Players = new List<Player>()
            {
                new HumanPlayer("Forehand"),
                new HumanPlayer("Middlehand"),
                new HumanPlayer("Rearhand")
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
            Bidding();
            Declarer.SwapWithSkat(Skat);
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

        private void Bidding()
        {
            var firstBidWinner = PlayerBidsOtherPlayer(Players[1], Players[0]);
            Declarer = PlayerBidsOtherPlayer(Players[2], firstBidWinner);
        }

        private Player PlayerBidsOtherPlayer(Player bidder, Player listener)
        {
            while (true)
            {
                var bid = bidder.Bid(BidValue);
                if (bid == -1)
                    return listener;
                BidValue = bid;
                var callsBid = listener.Listen(BidValue);
                if (!callsBid)
                    return bidder;
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
        public RuleModifiers Modifiers;
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

    public enum RuleModifiers
    {
        Normal,
        Hand,
        Schneider,
        SchneiderAngesagt, // implies Hand
        Schwarz,
        SchwarzAngesagt, // implies Hand
        Open, // implies Hand
        HandOpen // implies Null gamemode
    }
}