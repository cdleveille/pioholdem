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
            Player opp = game.players[game.GetNextPosition(game.actingIndex)];

            //Console.WriteLine();
            //Console.WriteLine(me.name + "'s hole cards: |" + me.holeCards[0] + "|" + me.holeCards[1] + "|");

            string holeCards = eval.ClassifyHoleCards(me.holeCards);

            if (game.isPreflop)
            {
                // Use push/fold strategy for under 20BB
                if (game.effectiveStack <= 20.0)
                {
                    if (game.actionCount == 0)
                    {
                        // BU first to act
                        return game.effectiveStack <= pf.pushFold_shove[holeCards] ? me.stack : -1;
                    }
                    else if (game.actionCount == 1 && opp.stack == 0)
                    {
                        // BB facing with all in bet from BU
                        return game.effectiveStack <= pf.pushFold_call[holeCards] ? ValidateBetSize(game.betAmt - me.inFor, game) : -1;
                    }
                    else if (game.actionCount == 1)
                    {
                        // BB facing open raise/call from BU
                        return game.effectiveStack <= pf.pushFold_shove[holeCards] ? me.stack : -1;
                    }
                    else
                    {
                        return me.stack;
                    }
                }
                else
                {
                    // BU first to act
                    if (game.actionCount == 0)
                    {
                        if (pf.BUopen_100.Contains(holeCards))
                        {
                            // Open
                            return ValidateBetSize((int)(openMult * game.bbAmt) - me.inFor, game);
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
                            return ValidateBetSize((int)(oopRaiseMult * game.bbAmt) - me.inFor, game);
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
                            return ValidateBetSize((int)(oopRaiseMult * game.betAmt) - me.inFor, game);
                        }
                        else if (pf.BBcallOpen_100.Contains(holeCards))
                        {
                            // Call
                            return ValidateBetSize(game.betAmt - me.inFor, game);
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
                            return ValidateBetSize((int)(ipRaiseMult * game.betAmt) - me.inFor, game);
                        }
                        else if (pf.BUcall3bet_100.Contains(holeCards))
                        {
                            // Call
                            return ValidateBetSize(game.betAmt - me.inFor, game);
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
                    // BU facing BU 5bet
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
                    else
                    {
                        return -1;
                    }
                }
            }
            // Use FishAI strategy for postflop
            else
            {
                return fishAI.GetAction(game);
            }
        }

        private int ValidateBetSize(int bet, Game game)
        {
            Player me = game.players[game.actingIndex];
            if (bet > me.stack)
            {
                return me.stack;
            }
            return bet;
        }
    }
}
