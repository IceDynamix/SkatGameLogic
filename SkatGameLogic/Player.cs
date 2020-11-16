using System;
using System.Collections.Generic;

namespace SkatGameLogic
{
    public abstract class Player
    {
        public string Name;
        public CardCollection Hand;
        public CardCollection DiscardPile;
        public int GameScore;

        public abstract Card PlayCard(Game game);
        public abstract int Bid(int currentBid);
        public abstract bool Listen(int currentBid);
        public abstract bool SwapWithSkat();
        public abstract Rules SelectRules();
    }

    public class HumanPlayer : Player
    {
        public HumanPlayer(string name) => Name = name;

        public override Card PlayCard(Game game)
        {
            Console.WriteLine(Hand);
            Console.Write("Choose a card to play: ");

            while (true)
            {
                var success = int.TryParse(Console.ReadLine(), out int i);
                if (success && (i >= 0 || i < Hand.Cards.Count))
                    return Hand.Cards[i];
                else
                    Console.WriteLine("Invalid number, please try again");
            }
        }

        public override int Bid(int currentBid)
        {
            Console.WriteLine(Name + ": " + Hand);
            Console.WriteLine("Current Bid: " + currentBid);
            Console.WriteLine(
                "Please enter a number that is higher than the current bid (or enter nothing to pass)");
            while (true)
            {
                var input = Console.ReadLine();
                if (input == "") // pass
                    return -1;

                var success = int.TryParse(input, out int bid);
                if (!success)
                {
                    Console.WriteLine("Invalid number, please try again");
                }
                else if (bid <= currentBid)
                    Console.WriteLine("Bid too low, please try again");
                else
                    return bid;
            }
        }

        public override bool Listen(int currentBid)
        {
            Console.WriteLine("Current Bid: " + currentBid);
            Console.WriteLine("Go with the current bid? (Y/n)");
            return Console.ReadLine() != "n";
        }

        public override bool SwapWithSkat()
        {
            return false;
        }

        public override Rules SelectRules()
        {
            return new Rules();
        }
    }

    public class AiPlayer : Player
    {
        // Very basic default behavior
        public override Card PlayCard(Game game) => Hand.Cards[0];
        public override int Bid(int currentBid) => -1;
        public override bool Listen(int currentBid) => false;

        public override bool SwapWithSkat() => false;

        public override Rules SelectRules()
        {
            return new Rules()
            {
                GameMode = GameMode.Grand,
                Modifiers = RuleModifiers.Normal
            };
        }
    }
}