using System;

namespace PioHoldem
{
    class Client
    {
        static void Main(string[] args)
        {
            Player p1 = new HumanPlayer("Chris", 1000);
            Player p2 = new HumanPlayer("Aaron", 1000);
            //Player p2 = new BotPlayer("Bill", 1000);

            Player[] players = new Player[2];
            players[0] = p1;
            players[1] = p2;

            // Create and start a new game
            Game game = new Game(players, 5, 10);
            game.StartGame();

            //Card c1 = new Card(0, 12);
            //Card c2 = new Card(0, 0);
            //Card c3 = new Card(2, 1);
            //Card c4 = new Card(0, 2);
            //Card c5 = new Card(0, 3);
            //Card c6 = new Card(1, 5);
            //Card c7 = new Card(1, 4);

            //Card[] hand = new Card[7] { c1, c2, c3, c4, c5 , c6, c7 };
            //foreach (Card card in hand)
            //{
            //    Console.Write("|" + card);
            //}
            //Console.WriteLine("|");

            //ShowdownEvaluator eval = new ShowdownEvaluator();
            //Console.WriteLine(eval.HasStraight(hand));
            //Console.ReadLine();
        }
    }
}
