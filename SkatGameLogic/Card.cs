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

        public static List<Card> StandardSort(List<Card> originalCards, CardSuit trumpSuit = CardSuit.Clubs)
        {
            var sortedCards = new List<Card>();

            var jacks = originalCards.Where(c => c.CardNumber == CardNumber.Jack)
                .OrderBy(c => c.CardSuit)
                .ToList();
            sortedCards.AddRange(jacks);

            // Everything depends on the List.Distinct at the end so
            // we don't have to worry about adding duplicate cards
            var trumpSuitCards = originalCards.Where(c => c.CardSuit == trumpSuit)
                .OrderBy(c => c.CardNumber)
                .ToList();

            sortedCards.AddRange(trumpSuitCards);

            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
                sortedCards.AddRange(
                    originalCards.Where(c => c.CardSuit == suit)
                        .OrderBy(c => c.CardNumber)
                );

            return sortedCards.Distinct().ToList();
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
