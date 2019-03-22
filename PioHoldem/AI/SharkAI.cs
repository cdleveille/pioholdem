using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PioHoldem
{
    class SharkAI : DecisionEngine
    {
        private readonly double openMult = 2.5;
        private readonly double oopRaiseMult = 3.5;
        private readonly double ipRaiseMult = 3.0;
        private FishAI fishAI = new FishAI();
        PreflopLookups pf = new PreflopLookups();

        public override int GetAction(Game game)
        {
            Thread.Sleep(1000);

            Player me = game.players[game.actingIndex];

            Console.WriteLine();
            Console.WriteLine(me.name + "'s hole cards: |" + me.holeCards[0] + "|" + me.holeCards[1] + "|");

            string holeCards = eval.ClassifyHoleCards(me.holeCards);

            if (game.isPreflop)
            {
                // BU first to act
                if (game.actionCount == 0)
                {
                    if (pf.BUopen_100.Contains(holeCards))
                    {
                        // Open
                        return (int)(openMult * game.bbAmt) - me.inFor;
                    }
                    else
                    {
                        // Fold
                        return -1;
                    }
                }
                // BB facing BU limp (option)
                else if (game.actionCount == 1 && me.inFor == game.betAmt)
                {
                    if (pf.BB3bet_100.Contains(holeCards))
                    {
                        // Raise
                        return (int)(oopRaiseMult * game.bbAmt) - me.inFor;
                    }
                    else
                    {
                        // Check
                        return 0;
                    }
                }
                // BB facing BU open raise
                else if (game.actionCount == 1 && me.inFor < game.betAmt)
                {
                    if (pf.BB3bet_100.Contains(holeCards))
                    {
                        // 3bet
                        return (int)(oopRaiseMult * game.betAmt) - me.inFor;
                    }
                    else if (pf.BBcallOpen_100.Contains(holeCards))
                    {
                        // Call
                        return game.betAmt - me.inFor;
                    }
                    else
                    {
                        // Fold
                        return -1;
                    }
                }
                // BU facing BB 3bet
                else if (game.actionCount == 2 && me.inFor < game.betAmt && me.inFor > game.bbAmt)
                {
                    if (pf.BU4bet_100.Contains(holeCards))
                    {
                        // 4bet
                        return (int)(ipRaiseMult * game.betAmt) - me.inFor;
                    }
                    else if (pf.BUcall3bet_100.Contains(holeCards))
                    {
                        // Call
                        return game.betAmt - me.inFor;
                    }
                    else
                    {
                        // Fold
                        return -1;
                    }
                }
                // BB facing BU 4bet
                else if (game.actionCount == 3)
                {
                    if (pf.BB5betShove_100.Contains(holeCards))
                    {
                        // 5bet shove
                        return me.stack - me.inFor;
                    }
                    else
                    {
                        // Fold
                        return -1;
                    }
                }
                // BU facing BU 5bet shove
                else if (game.actionCount == 4)
                {
                    if (pf.BUcallShove_100.Contains(holeCards))
                    {
                        // Call
                        return game.betAmt - me.inFor;
                    }
                    else
                    {
                        return -1;
                    }
                }

                return 0;
            }
            // Use FishAI strategy
            else
            {
                return fishAI.GetAction(game);
            }
        }
    }
}
