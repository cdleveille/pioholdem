using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PioHoldem
{
    abstract class DecisionEngine
    {
        public static Random rng;
        public static HandEvaluator eval;

        public DecisionEngine()
        {
            rng = new Random();
            eval = new HandEvaluator();
        }

        public abstract int GetAction(Game game);
    }
}
