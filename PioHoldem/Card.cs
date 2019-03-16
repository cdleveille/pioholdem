using System;

namespace PioHoldem
{
    class Card
    {
        public int suit;
        public int value;

        // Create a new Card of the specified suit and value
        public Card(int suit, int value)
        {
            this.suit = suit;
            this.value = value;
        }

        override public string ToString()
        {
            try
            {
                string toReturn = "";
                if (value == 0) { toReturn += "2"; }
                else
                if (value == 1) { toReturn += "3"; }
                else
                if (value == 2) { toReturn += "4"; }
                else
                if (value == 3) { toReturn += "5"; }
                else
                if (value == 4) { toReturn += "6"; }
                else
                if (value == 5) { toReturn += "7"; }
                else
                if (value == 6) { toReturn += "8"; }
                else
                if (value == 7) { toReturn += "9"; }
                else
                if (value == 8) { toReturn += "T"; }
                else
                if (value == 9) { toReturn += "J"; }
                else
                if (value == 10) { toReturn += "Q"; }
                else
                if (value == 11) { toReturn += "K"; }
                else
                if (value == 12) { toReturn += "A"; }
                else { throw new Exception("Invalid card value! (" + value + ")"); }

                if (suit == 0) { toReturn += "c"; }
                else
                if (suit == 1) { toReturn += "d"; }
                else
                if (suit == 2) { toReturn += "h"; }
                else
                if (suit == 3) { toReturn += "s"; }
                else { throw new Exception("Invalid card suit! (" + suit + ")"); }

                return toReturn;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
