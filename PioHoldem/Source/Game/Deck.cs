using System;

namespace PioHoldem
{
    class Deck
    {
        private Card[] cards;
        private int topIndex;
        private Random rng;

        // Create a new Deck of 52 Cards
        public Deck()
        {
            cards = new Card[52];
            for (int suit = 0; suit < 4; suit++)
            {
                for (int value = 0; value < 13; value++)
                    cards[13 * suit + value] = new Card(suit, value);
            }
            topIndex = 0;
            rng = new Random();
        }

        // Randomize the order of the cards and reset the topIndex
        public void Shuffle()
        {
            for (int reps = 0; reps < 100; reps++)
            {
                for (int i = 0; i < cards.Length; i++)
                {
                    int j = rng.Next(cards.Length);
                    Card iCard = cards[i];
                    Card jCard = cards[j];
                    cards[i] = jCard;
                    cards[j] = iCard;
                }
            }
            topIndex = 0;
        }

        // Deal one Card
        public Card Deal()
        {
            topIndex++;
            return cards[topIndex - 1];
        }

        // Deal multiple Cards
        public Card[] Deal(int count)
        {
            Card[] toReturn = new Card[count];
            for (int i = 0; i < count; i++)
                toReturn[i] = Deal();
            return toReturn;
        }

        // Discard the top Card on the Deck
        public void Burn()
        {
            topIndex++;
        }

        public override string ToString()
        {
            string toReturn = "";
            foreach (Card card in cards)
                toReturn += "|" + card;
            return toReturn + "|";
        }
    }
}
