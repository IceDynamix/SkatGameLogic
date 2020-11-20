using System;
using System.Collections.Generic;
using System.Linq;

namespace SkatGameLogic
{
    public static class Cards
    {
        public static List<Card> GenerateSkatDeck()
        {
            var cards = new List<Card>();
            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            foreach (CardNumber number in Enum.GetValues(typeof(CardNumber)))
                cards.Add(new Card {CardNumber = number, CardSuit = suit});

            return cards;
        }

        public static List<Card> StandardSort(List<Card> originalCards, CardSuit trumpSuit = CardSuit.Clubs,
            CardSuit secondarySuit = CardSuit.Clubs)
        {
            var baseValue = Enum.GetNames(typeof(CardNumber)).Length;
            return originalCards.OrderByDescending(c =>
            {
                var mainPriority = 0;
                if (c.CardNumber == CardNumber.Jack)
                    mainPriority = 3;
                else if (c.CardSuit == trumpSuit)
                    mainPriority = 2;
                else if (c.CardSuit == secondarySuit)
                    mainPriority = 1;

                var suitPriority = Enum.GetNames(typeof(CardSuit)).Length - ((int) c.CardSuit);
                var numberPriority = baseValue - ((int) c.CardNumber);

                return mainPriority * baseValue * baseValue + suitPriority * baseValue + numberPriority;
            }).ToList();
        }

        public static List<Card> NullSort(List<Card> originalCards)
        {
            var baseValue = Enum.GetNames(typeof(CardNumber)).Length;
            return originalCards.OrderByDescending(c =>
                Enum.GetNames(typeof(CardSuit)).Length - ((int) c.CardSuit) * baseValue * baseValue +
                Card.PointValues[c.CardNumber] * baseValue +
                baseValue - ((int) c.CardNumber)
            ).ToList();
        }

        public static List<Card> Shuffle(List<Card> originalCards)
        {
            var rand = new Random();
            return originalCards.OrderBy(c => rand.Next()).ToList();
        }

        public static void PrintColored(List<Card> cards, bool printIndices = false)
        {
            if (printIndices)
                Console.WriteLine(String.Join('\t', Enumerable.Range(0, cards.Count)));
            foreach (var card in cards)
            {
                card.PrintColored();
                Console.Write("\t");
            }

            Console.Write("\n");
        }
    }

    public struct Card
    {
        public CardSuit CardSuit { get; set; }
        public CardNumber CardNumber { get; set; }

        public static readonly Dictionary<CardSuit, ConsoleColor> SuitColors = new Dictionary<CardSuit, ConsoleColor>
        {
            {CardSuit.Clubs, ConsoleColor.White},
            {CardSuit.Spades, ConsoleColor.Green},
            {CardSuit.Hearts, ConsoleColor.Red},
            {CardSuit.Diamonds, ConsoleColor.Yellow}
        };

        public static readonly Dictionary<CardSuit, char> SuitUnicode = new Dictionary<CardSuit, char>
        {
            {CardSuit.Clubs, '♣'},
            {CardSuit.Spades, '♠'},
            {CardSuit.Hearts, '♥'},
            {CardSuit.Diamonds, '♦'}
        };

        public static readonly Dictionary<CardNumber, String> NumberUnicode = new Dictionary<CardNumber, String>
        {
            {CardNumber.Jack, "J"},
            {CardNumber.Ace, "A"},
            {CardNumber.Ten, "10"},
            {CardNumber.King, "K"},
            {CardNumber.Queen, "Q"},
            {CardNumber.Nine, "9"},
            {CardNumber.Eight, "8"},
            {CardNumber.Seven, "7"},
        };

        public static readonly Dictionary<CardNumber, int> PointValues = new Dictionary<CardNumber, int>
        {
            {CardNumber.Jack, 2},
            {CardNumber.Ace, 11},
            {CardNumber.Ten, 10},
            {CardNumber.King, 4},
            {CardNumber.Queen, 3},
            {CardNumber.Nine, 0},
            {CardNumber.Eight, 0},
            {CardNumber.Seven, 0},
        };
        
        public static readonly List<CardNumber> NullOrder = new List<CardNumber>()
        {
            CardNumber.Ace,
            CardNumber.King,
            CardNumber.Queen,
            CardNumber.Jack,
            CardNumber.Ten,
            CardNumber.Nine,
            CardNumber.Eight,
            CardNumber.Seven
        };

        public void PrintColored()
        {
            var previous = Console.ForegroundColor;
            Console.ForegroundColor = SuitColors[CardSuit];
            Console.Write(this);
            Console.ForegroundColor = previous;
        }

        public override string ToString() => $"{SuitUnicode[CardSuit]}{NumberUnicode[CardNumber]}";
    }

    public enum CardSuit
    {
        Clubs,
        Spades,
        Hearts,
        Diamonds
    }

    public enum CardNumber
    {
        Jack,
        Ace,
        Ten,
        King,
        Queen,
        Nine,
        Eight,
        Seven
    }
}