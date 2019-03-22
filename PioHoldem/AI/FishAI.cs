using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PioHoldem
{
    class FishAI : DecisionEngine
    {
        public override int GetAction(Game game)
        {
            Thread.Sleep(1000);

            int action;
            if (game.betAmt == 0)
            {
                // There is no active bet
                // Fold[1] Check[2] Bet[4]
                action = rng.Next(3);
                if (action == 0)
                {
                    // Check
                    return 0;
                }
                else
                {
                    // Bet 1/2 pot
                    if (game.pot / 2 < game.players[game.actingIndex].stack)
                    {
                        return game.pot / 2;
                    }
                    else
                    {
                        return game.players[game.actingIndex].stack;
                    }
                }
            }
            else if (game.betAmt == game.players[game.actingIndex].inFor)
            {
                // BB option
                // Fold[1] Check[2] Raise[5]
                action = rng.Next(4);
                if (action == 0)
                {
                    // Raise 3x
                    if ((3 * game.betAmt) - game.players[game.actingIndex].inFor < game.players[game.actingIndex].stack)
                    {
                        return (3 * game.betAmt) - game.players[game.actingIndex].inFor;
                    }
                    else
                    {
                        return game.players[game.actingIndex].stack;
                    }
                }
                else
                {
                    // Check
                    return 0;
                }
            }
            else if (game.players[game.GetPreviousPosition(game.actingIndex)].stack == 0)
            {
                // Facing an all-in bet
                // Call
                if (game.betAmt - game.players[game.actingIndex].inFor >= game.players[game.actingIndex].stack)
                {
                    return game.players[game.actingIndex].stack;
                }
                else
                {
                    return game.betAmt - game.players[game.actingIndex].inFor;
                }
            }
            else
            {
                // There is an active bet
                // Fold[1] Call[3] Raise[5]
                action = rng.Next(10);
                if (action < 0)
                {
                    // Fold
                    return -1;
                }
                else if (action < 5)
                {
                    // Call
                    if (game.betAmt - game.players[game.actingIndex].inFor >= game.players[game.actingIndex].stack)
                    {
                        return game.players[game.actingIndex].stack;
                    }
                    else
                    {
                        return game.betAmt - game.players[game.actingIndex].inFor;
                    }
                }
                else
                {
                    // Raise 3x
                    if ((3 * game.betAmt) - game.players[game.actingIndex].inFor >= game.players[game.actingIndex].stack)
                    {
                        return game.players[game.actingIndex].stack;
                    }
                    else
                    {
                        return (3 * game.betAmt) - game.players[game.actingIndex].inFor;
                    }
                }
            }
        }
    }
}
