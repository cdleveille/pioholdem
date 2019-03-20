using System;

namespace PioHoldem
{
    class Client
    {
        static void Main(string[] args)
        {
            FishAI fishAI = new FishAI();

            Player p1 = new BotPlayer("Chris", 1000, fishAI);
            //Player p2 = new HumanPlayer("Aaron", 1000);
            Player p2 = new BotPlayer("FishAI", 1000, fishAI);

            Player[] players = new Player[2];
            players[0] = p1;
            players[1] = p2;

            // Create and start a new game
            Game game = new Game(players, 5, 10);
            game.StartGame();

            Card c1 = new Card(0, 5);
            Card c2 = new Card(1, 0);
            Card c3 = new Card(2, 9);
            Card c4 = new Card(3, 3);
            Card c5 = new Card(0, 0);
            Card c6 = new Card(1, 8);
            Card c7 = new Card(2, 11);

            //Card[] hand = new Card[7] { c1, c2, c3, c4, c5, c6, c7 };
            //foreach (Card card in hand)
            //{
            //    Console.Write("|" + card);
            //}
            //Console.WriteLine("|");

            //HandEvaluator eval = new HandEvaluator();
            //eval.SortByValueDescending(hand);
            //Console.WriteLine(eval.Ev(hand));
            //Console.ReadLine();


            // Testing HandEvaluator
            //Deck deck = new Deck();
            //HandEvaluator eval = new HandEvaluator();
            //deck.Shuffle();
            //Card[] board = deck.Deal(5);

            //Console.Write("Board: ");
            //foreach (Card card in board)
            //{
            //    if (card != null)
            //    {
            //        Console.Write("|" + card);
            //    }
            //    else
            //    {
            //        Console.Write("|  ");
            //    }
            //}
            //Console.WriteLine("|");

            //players[0].holeCards = deck.Deal(2);
            //players[1].holeCards = deck.Deal(2);

            //eval.EvaluateHands(players, board);

            //Console.ReadLine();
        }
    }
}
