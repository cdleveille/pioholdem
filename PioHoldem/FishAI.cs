using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PioHoldem
{
    class FishAI : DecisionEngine
    {
        public override int GetAction(Game game)
        {
            if (game.betAmt == 0)
            {
                // Fold[1] Check[2] Bet[4]
                rng.Next();
            }
            else if (game.betAmt == game.players[game.actingIndex].inFor)
            {
                // Fold[1] Check[2] Raise[5]
            }
            else// if (game.currentBetAmt > inForOnCurrentStreet)
            {
                // Fold[1] Call[3] Raise[5]
            }
            return 0;
        }
    }
}
