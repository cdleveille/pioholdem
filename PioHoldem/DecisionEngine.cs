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

        public DecisionEngine()
        {
            rng = new Random();
        }

        public abstract int GetAction(Game game);
    }
}
