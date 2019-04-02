using System;
using System.Linq;

namespace PioHoldem
{
    class HumanPlayer : Player
    {
        public HumanPlayer(string name, int startingStack) : base(name, startingStack){}

        public override int GetAction(Game game)
        {
            Console.WriteLine();
            Console.WriteLine(name + "'s hole cards: |" + holeCards[0] + "|" + holeCards[1] + "|");
            int[] validActions;
            string options;

            // No active bet
            if (game.betAmt == 0)
            {
                options = "Check[2] Bet[4]";
                validActions = new int[] { 2, 4 };
            }
            // BB option
            else if (game.betAmt == inFor)
            {
                options = "Check[2] Raise[5]";
                validActions = new int[] { 2, 5 };
            }
            // Facing an all-in bet/raise from opponent
            else if (game.players[game.GetPreviousPosition(game.actingIndex)].stack == 0)
            {
                options = "Fold[1] Call[3]";
                validActions = new int[] { 1, 3 };
            }
            // Facing an active bet (not all-in)
            else
            {
                options = "Fold[1] Call[3] Raise[5]";
                validActions = new int[] { 1, 3, 5 };
            }

            return GetInput(game, validActions, options);
        }

        private int GetInput(Game game, int[] validActions, string options)
        {
            int input;
            try
            {
                Console.WriteLine(name + "'s Action (" + options + "): ");
                input = int.Parse(Console.ReadLine());
                if (validActions.Contains(input))
                {
                    if (input == 1)
                        return -1;
                    else if (input == 2)
                        return 0;
                    else if (input == 3)
                    {
                        if (game.betAmt - inFor >= stack)
                            return stack;
                        else
                            return game.betAmt - inFor;
                    }
                    else if (input == 4)
                    {
                        int amtInput = GetAmtInput(game.betAmt, game.prevBetAmount, game.bbAmt, "Bet amount:");
                        if (amtInput >= stack)
                            return stack;
                        else
                            return amtInput;
                    }
                    else if (input == 5)
                    {
                        int amtInput = GetAmtInput(game.betAmt, game.prevBetAmount, game.bbAmt, "Raise to amount:");
                        if (amtInput - inFor >= stack)
                            return stack;
                        else
                            return amtInput - inFor;
                    }
                    else
                        throw new Exception();
                }
                else
                    throw new Exception();
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid input!");
                return GetInput(game, validActions, options);
            }
        }

        private int GetAmtInput(int betAmt, int prevBetAmt, int minBet, string prompt)
        {
            try
            {
                Console.WriteLine(prompt);
                int amtInput = int.Parse(Console.ReadLine());
                if (betAmt > 0 && amtInput < betAmt + (betAmt - prevBetAmt))
                {
                    Console.WriteLine("Invalid raise! Minimum amount is " + (betAmt + (betAmt - prevBetAmt)));
                    return GetAmtInput(betAmt, prevBetAmt, minBet, prompt);
                }
                else if (betAmt == 0 && amtInput < minBet)
                {
                    Console.WriteLine("Invalid bet! Minimum amount is " + minBet);
                    return GetAmtInput(betAmt, prevBetAmt, minBet, prompt);
                }
                else
                    return amtInput;

            }
            catch (Exception ex)
            {
                if (ex.Message == "Value was either too large or too small for an Int32.")
                    return 2 * stack;
                else
                {
                    Console.WriteLine("Invalid input!");
                    return GetAmtInput(betAmt, prevBetAmt, minBet, prompt);
                }
            }
        }
    }
}
