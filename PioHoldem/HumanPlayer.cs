using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PioHoldem
{
    class HumanPlayer : Player
    {
        public HumanPlayer(string name, int startingStack) : base(name, startingStack)
        {

        }

        public override int GetAction(Game game)
        {
            Console.WriteLine(name + "'s hole cards: |" + holeCards[0] + "|" + holeCards[1] + "|");
            int[] validActions;
            string options;

            if (game.betAmt == 0)
            {
                options = "Fold[1] Check[2] Bet[4]";
                validActions = new int[] { 1, 2, 4 };
            }
            else if (game.betAmt == inFor)
            {
                options = "Fold[1] Check[2] Raise[5]";
                validActions = new int[] { 1, 2, 5 };
            }
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
                    {
                        return -1;
                    }
                    else if (input == 2)
                    {
                        return 0;
                    }
                    else if (input == 3)
                    {
                        if (game.betAmt - inFor >= stack)
                        {
                            return stack;
                        }
                        else
                        {
                            return game.betAmt - inFor;
                        }
                    }
                    else if (input == 4)
                    {
                        int amtInput = GetAmtInput(game.betAmt);
                        if (amtInput >= stack)
                        {
                            return stack;
                        }
                        else
                        {
                            return amtInput;
                        }
                    }
                    else if (input == 5)
                    {
                        int amtInput = GetAmtInput(game.betAmt);
                        if (amtInput - inFor >= stack)
                        {
                            return stack;
                        }
                        else
                        {
                            return amtInput - inFor;
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid input!");
                return GetInput(game, validActions, options);
            }
        }

        private int GetAmtInput(int betAmt)
        {
            try
            {
                Console.WriteLine("Amount: ");
                int amtInput = int.Parse(Console.ReadLine());
                if (amtInput < (2 * betAmt))
                {
                    Console.WriteLine("Raise must be at least 2x the bet amount!");
                    return GetAmtInput(betAmt);
                }
                else
                {
                    return amtInput;
                }

            }
            catch (Exception ex)
            {
                if (ex.Message == "Value was either too large or too small for an Int32.")
                {
                    return 2 * stack;
                }
                else
                {
                    Console.WriteLine("Invalid input!");
                    return GetAmtInput(betAmt);
                }
            }
        }
    }
}
