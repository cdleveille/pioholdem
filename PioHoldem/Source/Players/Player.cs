using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PioHoldem
{
    abstract class Player
    {
        public string name;
        public int stack, startingStack, inFor, winCount;
        public bool folded;
        public bool busted;
        public Card[] holeCards;

        // Create a new Player with the specified name and starting stack amount
        public Player(string name, int startingStack)
        {
            this.name = name;
            stack = startingStack;
            this.startingStack = startingStack;
            winCount = 0;
        }

        // Get the Player's action
        public abstract int GetAction(Game game);

        public override string ToString()
        {
            return name + "[" + (stack == 0 ? "*ALL IN*" : stack.ToString()) + "]";
        }
    }
}
