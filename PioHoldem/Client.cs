using System;

namespace PioHoldem
{
    class Client
    {
        static void Main(string[] args)
        {
            Deck deck = new Deck();
            deck.Shuffle();
            Console.WriteLine(deck.ToString());

            Player p1 = new HumanPlayer("Chris", 100000000);
            Console.WriteLine(p1.ToString());

            Console.ReadLine();
        }
    }
}
