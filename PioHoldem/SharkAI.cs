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
        private readonly double pfOpenMultiplier = 2.5;
        private readonly double pfOOPRaiseMultiplier = 3.5;
        private readonly double pfIPRaiseMultiplier = 3.0;
        private FishAI fishAI = new FishAI();

        public override int GetAction(Game game)
        {
            Thread.Sleep(1000);

            Player me = game.players[game.actingIndex];
            string holeCards = eval.ClassifyHoleCards(me.holeCards);

            if (game.isPreflop)
            {
                // BU first to act
                if (game.actionCount == 0)
                {

                }
                // BB facing BU limp (option)
                else if (game.actionCount == 1 && me.inFor == game.betAmt)
                {

                }
                // BB facing BU open raise
                else if (game.actionCount == 1 && me.inFor < game.betAmt)
                {

                }
                // BU facing BB raise after limping
                else if (game.actionCount == 2 && me.inFor < game.betAmt && me.inFor == game.bbAmt)
                {

                }
                // BU facing BB 3bet after open raise
                else if (game.actionCount == 2 && me.inFor < game.betAmt && me.inFor > game.bbAmt)
                {

                }
                // BB facing BU 4bet
                else if (game.actionCount == 3)
                {

                }
                // BU facing BU 5bet
                else if (game.actionCount == 4)
                {

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
