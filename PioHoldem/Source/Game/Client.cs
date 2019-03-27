using System;

namespace PioHoldem
{
    class Client
    {
        static void Main(string[] args)
        {
            FishAI fish = new FishAI();
            SharkAI shark = new SharkAI();

            Player p1 = new HumanPlayer("Human", 200);
            Player p2 = new BotPlayer("SharkAI", 200, shark);
            Player[] players = new Player[] { p1, p2 };

            // Create and start a new game
            //while (true)
            //{
            //    Game game = new Game(players, 5, 10, 1000);
            //    game.StartGame();
            //}

            //Showdown!
            //Board: | As | 3h | 7d | 6h | 6d |
            //Human shows | 4c | Qd | *Pair *
            //SharkAI shows | Jh | 8h | *Pair *
            //Chop pot!
            //Human wins 85
    

            Card c1 = new Card(3, 12);
            Card c2 = new Card(2, 1);
            Card c3 = new Card(1, 5);
            Card c4 = new Card(2, 4);
            Card c5 = new Card(1, 4);

            Card c6 = new Card(2, 9);
            Card c7 = new Card(2, 6);

            Card[] hand = new Card[7] { c1, c2, c3, c4, c5, c6, c7 };
            foreach (Card card in hand)
            {
                Console.Write("|" + card);
            }
            Console.WriteLine("|");

            HandEvaluator eval = new HandEvaluator();
            Console.WriteLine(eval.GetHandValue(hand));
            Console.ReadLine();


            // Testing HandEvaluator
            //Deck deck = new Deck();
            //HandEvaluator eval = new HandEvaluator();

            //for (int i = 0; i < 50; i++)
            //{
            //    deck.Shuffle();
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

            //    players[0].holeCards = deck.Deal(2);
            //    players[1].holeCards = deck.Deal(2);

            //    Console.WriteLine(eval.ClassifyHoleCards(players[0].holeCards));
            //    Console.WriteLine(eval.ClassifyHoleCards(players[1].holeCards));


            //}
            //Console.ReadLine();

        }
    }
}
