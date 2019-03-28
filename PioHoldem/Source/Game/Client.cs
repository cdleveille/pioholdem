using System;

namespace PioHoldem
{
    class Client
    {
        static void Main(string[] args)
        {
            FishAI fish = new FishAI();
            SharkAI shark = new SharkAI();

            Player human1 = new HumanPlayer("Human1", 200);
            Player human2 = new HumanPlayer("Human2", 200);
            Player bot1 = new BotPlayer("Shark1", 200, shark);
            Player bot2 = new BotPlayer("Shark2", 200, shark);

            Player[] players;

            int gameMode = SelectGameMode();
            if (gameMode == 1)
            {
                players = new Player[] { human1, bot1 };
            }
            else if (gameMode == 2)
            {
                players = new Player[] { bot1, bot2 };
            }
            else
            {
                players = new Player[] { human1, human2 };
            }

            // Create and start a new game
            while (true)
            {
                Game game = new Game(players, 5, 10, 1000);
                game.StartGame();
            }

            //Card c1 = new Card(3, 12);
            //Card c2 = new Card(2, 12);
            //Card c3 = new Card(1, 1);
            //Card c4 = new Card(2, 2);
            //Card c5 = new Card(1, 4);

            //Card c6 = new Card(1, 9);
            //Card c7 = new Card(2, 9);

            //Card c8 = new Card(3, 10);
            //Card c9 = new Card(2, 10);
            //Card c10 = new Card(1, 3);
            //Card c11 = new Card(2, 2);
            //Card c12 = new Card(1, 0);

            //Card c13 = new Card(1, 11);
            //Card c14 = new Card(2, 11);

            //Card[] hand = new Card[7] { c1, c2, c3, c4, c5, c6, c7 };
            //Card[] hand2 = new Card[7] { c8, c9, c10, c11, c12, c13, c14 };
            //foreach (Card card in hand)
            //{
            //    Console.Write("|" + card);
            //}
            //Console.WriteLine("|");
            //foreach (Card card in hand2)
            //{
            //    Console.Write("|" + card);
            //}
            //Console.WriteLine("|");

            //HandEvaluator eval = new HandEvaluator();
            //Console.WriteLine(eval.GetHandValue(hand));
            //Console.WriteLine(eval.GetHandValue(hand2));
            //Console.ReadLine();

        }

        private static int SelectGameMode()
        {
            Console.WriteLine("[1] Human vs Bot");
            Console.WriteLine("[2] Bot vs Bot");
            Console.WriteLine("[3] Human vs Human");

            return GetGameModeInput();
        }

        private static int GetGameModeInput()
        {
            try
            {
                int input = int.Parse(Console.ReadLine());
                if (input >=1 && input <= 3)
                {
                    return input;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid input!");
                return GetGameModeInput();
            }
        }
    }
}
