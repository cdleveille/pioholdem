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

        public BotPlayer(string name, int startingStack, DecisionEngine decisionEngine) : base(name, startingStack)
        {
            this.decisionEngine = decisionEngine;
        }

        public override int GetAction(Game game)
        {
            return decisionEngine.GetAction(game);
        }
    }
}
