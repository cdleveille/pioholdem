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

            if (game.currentBetAmt == 0)
            {
                Console.WriteLine("Fold[1] Check[2] Bet[4]");
            }
            else if (game.currentBetAmt == inForOnCurrentStreet)
            {
                Console.WriteLine("Fold[1] Check[2] Raise[5]");
            }
            else if (game.currentBetAmt > inForOnCurrentStreet)
            {
                Console.WriteLine("Fold[1] Call[3] Raise[5]");
            }

            int input = int.Parse(Console.ReadLine());

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
                return game.currentBetAmt - inForOnCurrentStreet;
            }
            else if (input == 4)
            {
                Console.WriteLine("Amount: ");
                int amtInput = int.Parse(Console.ReadLine());
                return amtInput;
            }
            else if (input == 5)
            {
                Console.WriteLine("Amount (total): ");
                int amtInput = int.Parse(Console.ReadLine());
                return amtInput - inForOnCurrentStreet;
            }
            else
            {
                return -1;
            }
        }
    }
}
