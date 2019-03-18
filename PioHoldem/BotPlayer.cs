using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PioHoldem
{
    class BotPlayer : Player
    {
        private DecisionEngine decisionEngine;

        public BotPlayer(string name, int startingStack) : base(name, startingStack)
        {
            decisionEngine = new FishAI();
        }

        public override int GetAction(Game game)
        {
            return decisionEngine.GetAction(game);
        }
    }
}
