using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SkatGameLogic.Card
{
    public class CardCollection
    {
        public List<Card> Cards { get; }

        public CardCollection(List<Card> cards) => Cards = cards;

        public static CardCollection GenerateSkatDeck()
        {
            var cards = new List<Card>();
            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            foreach (CardNumber number in Enum.GetValues(typeof(CardNumber)))
                cards.Add(new Card {CardNumber = number, CardSuit = suit});

            return new CardCollection(cards);
        }

        public void StandardSort(CardSuit trumpSuit = CardSuit.Clubs)
        {
            var sortedCards = new List<Card>();

            var jacks = Cards.Where(c => c.CardNumber == CardNumber.Jack)
                .OrderBy(c => c.CardSuit)
                .ToList();
            sortedCards.AddRange(jacks);

            // Everything depends on the List.Distinct at the end so
            // we don't have to worry about adding duplicate cards
            var trumpSuitCards = Cards.Where(
                c => c.CardSuit == trumpSuit).ToList();
            sortedCards.AddRange(trumpSuitCards);

            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
                sortedCards.AddRange(Cards.Where(c => c.CardSuit == suit));

            Cards.Clear();
            Cards.AddRange(sortedCards.Distinct());
        }

        public CardCollection Take(int n)
        {
            n = Math.Min(n, Cards.Count); // Can't take more than there is
            var takenCards = new CardCollection(Cards.Take(n).ToList());
            Cards.RemoveRange(0, n);
            return takenCards;
        }

        public void Shuffle()
        {
            var rand = new Random();
            var shuffledCards = Cards.OrderBy(c => rand.Next()).ToList();
            Cards.Clear();
            Cards.AddRange(shuffledCards);
        }

        public override string ToString() => String.Join('\n', Cards.Select(c => c.ToString()));
    }

    public class Card
    {
        public CardSuit CardSuit { get; set; }
        public CardNumber CardNumber { get; set; }
        public override string ToString() => $"{CardNumber} of {CardSuit}";
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