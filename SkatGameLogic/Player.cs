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

        public Player(string name) => Name = name;

        public abstract int? Bid(int currentBid);
        public abstract bool Listen(int currentBid);
        public abstract bool SwapWithSkat(CardCollection skat);
        public abstract Rules SelectRules(bool lookedAtSkat);
        public abstract Card PlayCard(Game game);
    }

    public class HumanPlayer : Player
    {
        public HumanPlayer(string name) : base(name)
        {
        }

        public override int? Bid(int currentBid)
        {
            Console.WriteLine(Name + ": " + Hand);
            Console.WriteLine("Current Bid: " + currentBid);
            Console.WriteLine("Please enter a number that is higher than the current bid (or enter nothing to pass)");
            while (true)
            {
                var bid = ConsoleLineUtils.GetInt();
                if (bid != null && bid <= currentBid)
                    Console.WriteLine("Bid has to be higher than the current bid (enter nothing to pass)");
                else
                    return bid;
            }
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
                Console.WriteLine("Skat:\t" + skat);
                Console.WriteLine(String.Join('\t', Enumerable.Range(0, Hand.Cards.Count)));
                Console.WriteLine(Hand);
            }

            var swapNthSkatCard = 0;
            Console.WriteLine("------------------------------------");
            Console.WriteLine("Swap skat card " + swapNthSkatCard);
            printStatus();
            while (true)
            {
                int? i;
                while (true)
                {
                    Console.WriteLine("Enter an index of number to swap (enter nothing to go to next skat card)");
                    i = ConsoleLineUtils.GetInt();
                    if (i == null || i >= 0 && i < Hand.Cards.Count)
                        break;
                    Console.WriteLine("Number not in range");
                }

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

        public override Rules SelectRules(bool lookedAtSkat)
        {
            var gameModeIndex = ConsoleLineUtils.SelectFromList(
                "Select a game mode",
                Enum.GetNames(typeof(GameMode)).ToList()
            );
            var gameMode = (GameMode) gameModeIndex;

            var trumpSuit = CardSuit.Clubs;
            if (gameMode == GameMode.Color)
            {
                var colorIndex = ConsoleLineUtils.SelectFromList(
                    "Select a trump suit",
                    Enum.GetNames(typeof(CardSuit)).ToList()
                );
                trumpSuit = (CardSuit) colorIndex;
            }

            RuleModifier modifier;
            if (lookedAtSkat)
                modifier = RuleModifier.Normal;
            else
            {
                List<RuleModifier> validModifiers;
                if (gameMode == GameMode.Null)
                    validModifiers = new List<RuleModifier>()
                    {
                        RuleModifier.Hand,
                        RuleModifier.HandOpen
                    };
                else
                    validModifiers = new List<RuleModifier>()
                    {
                        RuleModifier.Hand,
                        RuleModifier.SchneiderAnnounced,
                        RuleModifier.SchwarzAnnounced,
                        RuleModifier.Open
                    };

                var modifierIndex = ConsoleLineUtils.SelectFromList(
                    "Select a game level",
                    validModifiers
                );
                modifier = validModifiers[modifierIndex];
            }

            return new Rules()
            {
                GameMode = gameMode,
                Modifier = modifier,
                TrumpSuit = trumpSuit
            };
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
                Console.WriteLine("Invalid number, please try again");
            }
        }
    }

    public class AiPlayer : Player
    {
        public AiPlayer(string name) : base(name)
        {
        }

        // Very basic default behavior

        public override int? Bid(int currentBid) => null;

        public override bool Listen(int currentBid) => false;

        public override bool SwapWithSkat(CardCollection skat) => false;

        public override Rules SelectRules(bool b)
        {
            return new Rules
            {
                GameMode = GameMode.Grand,
                Modifier = RuleModifier.Normal
            };
        }

        public override Card PlayCard(Game game) => Hand.Cards[0];
    }
}