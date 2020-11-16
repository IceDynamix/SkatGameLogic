using System;
using System.Collections.Generic;
using System.Linq;

namespace SkatGameLogic
{
    public abstract class Player
    {
        public string Name;
        public CardCollection Hand;
        public CardCollection DiscardPile;
        public int GameScore;

        public abstract int Bid(int currentBid);
        public abstract bool Listen(int currentBid);
        public abstract bool SwapWithSkat(CardCollection skat);
        public abstract Rules SelectRules();
        public abstract Card PlayCard(Game game);
    }

    public class HumanPlayer : Player
    {
        public HumanPlayer(string name) => Name = name;

        public override int Bid(int currentBid)
        {
            Console.WriteLine(Name + ": " + Hand);
            Console.WriteLine("Current Bid: " + currentBid);
            return ConsoleLineUtils.GetNullableInt(
                "Please enter a number that is higher than the current bid (or enter nothing to pass)",
                currentBid
            ) ?? -1;
        }

        public override bool Listen(int currentBid)
        {
            Console.WriteLine("Current Bid: " + currentBid);
            return ConsoleLineUtils.GetBool("Go with the current bid?");
        }

        public override bool SwapWithSkat(CardCollection skat)
        {
            var look = ConsoleLineUtils.GetBool("Look at skat?");
            if (!look)
                return false;

            void printStatus()
            {
                Console.WriteLine("---");
                Console.WriteLine("Skat:\t" + skat);
                Console.WriteLine("---");
                Console.WriteLine(String.Join('\t', Enumerable.Range(0, Hand.Cards.Count)));
                Console.WriteLine(Hand);
                Console.WriteLine("---");
            }

            var swapNthSkatCard = 0;
            Console.WriteLine("------------------------------------");
            Console.WriteLine("Swap skat card " + swapNthSkatCard);
            printStatus();
            while (true)
            {
                var i = ConsoleLineUtils.GetNullableInt(
                    "Enter index of card to swap (enter nothing to go to next skat card): ",
                    0, Hand.Cards.Count - 1
                );

                if (i != null)
                {
                    var temp = skat.Cards[swapNthSkatCard];
                    skat.Cards[swapNthSkatCard] = Hand.Cards[(int) i];
                    Hand.Cards[(int) i] = temp;
                    printStatus();
                }
                else
                {
                    if (++swapNthSkatCard == skat.Cards.Count)
                    {
                        Hand.StandardSort();
                        return true;
                    }

                    Console.WriteLine("------------------------------------");
                    Console.WriteLine("Swap skat card " + swapNthSkatCard);
                }
            }
        }

        public override Rules SelectRules()
        {
            return new Rules();
        }

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
    }

    public class AiPlayer : Player
    {
        // Very basic default behavior
        public override int Bid(int currentBid) => -1;
        public override bool Listen(int currentBid) => false;

        public override bool SwapWithSkat(CardCollection skat) => false;

        public override Rules SelectRules()
        {
            return new Rules()
            {
                GameMode = GameMode.Grand,
                Modifiers = RuleModifiers.Normal
            };
        }

        public override Card PlayCard(Game game) => Hand.Cards[0];
    }
}