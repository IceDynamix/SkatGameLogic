using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace SkatGameLogic
{
    public abstract class Player
    {
        public string Name;
        public List<Card> Hand;
        public List<Card> DiscardPile;
        public int GameScore;

        public Player(string name) => Name = name;

        public abstract int? Bid(int currentBid);
        public abstract bool Listen(int currentBid);
        public abstract bool SwapWithSkat(List<Card> skat);
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
            Cards.PrintColored(Hand);
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
            Cards.PrintColored(Hand);
            Console.WriteLine("Current Bid: " + currentBid);
            return ConsoleLineUtils.GetBool("Go with the current bid?");
        }

        public override bool SwapWithSkat(List<Card> skat)
        {
            void printStatus(int highlightNthSkatCard)
            {
                Console.Clear();

                Console.Write("Skat:\t");
                for (int i = 0; i < skat.Count; i++)
                {
                    if (i == highlightNthSkatCard)
                    {
                        var previousFg = Console.ForegroundColor;
                        Console.BackgroundColor = ConsoleColor.Gray;
                        skat[i].PrintColored();
                        Console.BackgroundColor = previousFg;
                    }
                    else
                        skat[i].PrintColored();

                    Console.Write("  ");
                }

                Console.Write("\n");
                Cards.PrintColored(Hand, true);
            }

            int? GetInt(string prompt)
            {
                while (true)
                {
                    Console.WriteLine(prompt);
                    var i = ConsoleLineUtils.GetInt();
                    if (i == null || i >= 0 && i < Hand.Count)
                        return i;
                    Console.WriteLine("Number not in range");
                }
            }

            void swapUntilBreak(int n, string comment)
            {
                while (true)
                {
                    printStatus(n);
                    var i = GetInt($"Swap nth hand card with Skat card {n} (enter nothing to {comment})");

                    if (i != null)
                    {
                        var temp = skat[n];
                        skat[n] = Hand[(int) i];
                        Hand[(int) i] = temp;
                    }
                    else
                        return;
                }
            }

            Cards.PrintColored(Hand);
            if (!ConsoleLineUtils.GetBool("Look at Skat?"))
                return false;

            swapUntilBreak(0, "go to next skat card");
            swapUntilBreak(1, "finish Skat swapping");
            return true;
        }

        public override Rules SelectRules(bool lookedAtSkat)
        {
            int SelectFromList<T>(string prompt, List<T> items)
            {
                Console.Clear();
                Cards.PrintColored(Hand);
                Console.WriteLine(prompt);
                for (int i = 0; i < items.Count; i++)
                    Console.WriteLine($"[{i}]: " + items[i]);
                while (true)
                {
                    var index = ConsoleLineUtils.GetInt();
                    if (index == null || index < 0 || index >= items.Count)
                        Console.WriteLine("Number not in range, please try again");
                    else
                        return (int) index;
                }
            }

            var rules = new Rules();

            var gameModeIndex = SelectFromList(
                "Select a game mode",
                Enum.GetNames(typeof(GameMode)).ToList()
            );
            rules.GameMode = (GameMode) gameModeIndex;

            if (rules.GameMode == GameMode.Color)
            {
                var colorIndex = SelectFromList(
                    "Select a trump suit",
                    Enum.GetNames(typeof(CardSuit)).ToList()
                );
                rules.TrumpSuit = (CardSuit) colorIndex;
            }

            if (lookedAtSkat)
                rules.Modifier = RuleModifier.Normal;
            else
            {
                List<RuleModifier> validModifiers;
                if (rules.GameMode == GameMode.Null)
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

                var modifierIndex = SelectFromList(
                    "Select a game level",
                    validModifiers
                );
                rules.Modifier = validModifiers[modifierIndex];
            }

            return rules;
        }

        public override Card PlayCard(Game game)
        {
            // TODO
            Console.WriteLine(Hand);
            Console.Write("Choose a card to play: ");

            while (true)
            {
                var success = int.TryParse(Console.ReadLine(), out int i);
                if (success && (i >= 0 || i < Hand.Count))
                    return Hand[i];
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

        public override bool SwapWithSkat(List<Card> skat) => false;

        public override Rules SelectRules(bool b)
        {
            return new Rules
            {
                GameMode = GameMode.Grand,
                Modifier = RuleModifier.Normal
            };
        }

        public override Card PlayCard(Game game) => Hand[0];
    }
}