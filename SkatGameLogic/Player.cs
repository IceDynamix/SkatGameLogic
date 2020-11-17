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
            Hand.PrintColored();
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
            Hand.PrintColored();
            Console.WriteLine("Current Bid: " + currentBid);
            return ConsoleLineUtils.GetBool("Go with the current bid?");
        }

        public override bool SwapWithSkat(CardCollection skat)
        {
            void printStatus(int highlightNthSkatCard)
            {
                Console.Clear();

                Console.Write("Skat:\t");
                for (int i = 0; i < skat.Cards.Count; i++)
                {
                    if (i == highlightNthSkatCard)
                    {
                        var previousFg = Console.ForegroundColor;
                        Console.BackgroundColor = ConsoleColor.Gray;
                        skat.Cards[i].PrintColored();
                        Console.BackgroundColor = previousFg;
                    }
                    else
                        skat.Cards[i].PrintColored();
                    Console.Write("  ");
                }

                Console.Write("\n");
                Console.WriteLine(String.Join('\t', Enumerable.Range(0, Hand.Cards.Count)));
                Hand.PrintColored();
            }

            int? GetInt(string prompt)
            {
                while (true)
                {
                    Console.WriteLine(prompt);
                    var i = ConsoleLineUtils.GetInt();
                    if (i == null || i >= 0 && i < Hand.Cards.Count)
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
                        var temp = skat.Cards[n];
                        skat.Cards[n] = Hand.Cards[(int) i];
                        Hand.Cards[(int) i] = temp;
                    }
                    else
                        return;
                }
            }

            Hand.PrintColored();
            if (!ConsoleLineUtils.GetBool("Look at Skat?"))
                return false;

            swapUntilBreak(0, "go to next skat card");
            swapUntilBreak(1, "finish Skat swapping");
            return true;
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
            // TODO
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