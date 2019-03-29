
namespace PioHoldem
{
    abstract class Player
    {
        public string name;
        public int stack, buyinAmt, inFor, winCount;
        public bool folded, busted, isAggressor;
        public Card[] holeCards;

        // Create a new player with the specified name and starting stack amount
        public Player(string name, int buyinAmt)
        {
            this.name = name;
            this.buyinAmt = buyinAmt;
            holeCards = new Card[2];
            winCount = 0;
        }

        // Get the player's action
        public abstract int GetAction(Game game);

        public override string ToString()
        {
            return name + "[" + (stack == 0 ? "*ALL IN*" : stack.ToString()) + "]";
        }
    }
}
