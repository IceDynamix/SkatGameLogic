using System;
using System.Linq;
using SkatGameLogic.Card;

namespace SkatGameLogic
{
    class Program
    {
        static void Main(string[] args)
        {
            var skatDeck = CardCollection.GenerateSkatDeck();
            Console.WriteLine(skatDeck);
            skatDeck.Shuffle();
            var cards = skatDeck.Take(10);
            Console.WriteLine("---");
            cards.StandardSort();
            Console.WriteLine(cards);
        }
    }
}
