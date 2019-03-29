using System;

namespace PioHoldem
{
    class Client
    {
        static void Main(string[] args)
        {
            FishAI fish = new FishAI();
            SharkAI shark = new SharkAI();

            int startingStack = 200;
            Player[] players;

            Console.WriteLine("[1] Human vs Bot\n[2] Bot vs Bot\n[3] Human vs Human");
            int gameMode = SelectGameMode();
            if (gameMode == 1)
            {
                players = new Player[] { new HumanPlayer("Chris", startingStack), new BotPlayer("SharkBot", startingStack, shark) };
            }
            else if (gameMode == 2)
            {
                players = new Player[] { new BotPlayer("GreatWhite", startingStack, shark), new BotPlayer("Hammerhead", startingStack, shark) };
            }
            else
            {
                players = new Player[] { new HumanPlayer("Chris", startingStack), new HumanPlayer("Aaron", startingStack) };
            }

            //UnitTests();

            // Create and start a new game
            while (true)
            {
                Game game = new Game(players, 5, 10, 1000);
                game.StartGame();
            }
        }

        // Get the user's input and return an int indicating the desired game mode
        private static int SelectGameMode()
        {
            try
            {
                int input = int.Parse(Console.ReadLine());
                if (input >= 1 && input <= 3)
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
                return SelectGameMode();
            }
        }

        private static void UnitTests()
        {
            Card c1 = new Card(3, 12);
            Card c2 = new Card(2, 12);
            Card c3 = new Card(1, 1);
            Card c4 = new Card(2, 2);
            Card c5 = new Card(1, 4);

            Card c6 = new Card(1, 9);
            Card c7 = new Card(2, 9);

            Card c8 = new Card(3, 10);
            Card c9 = new Card(2, 10);
            Card c10 = new Card(1, 3);
            Card c11 = new Card(2, 2);
            Card c12 = new Card(1, 0);

            Card c13 = new Card(1, 11);
            Card c14 = new Card(2, 11);

            Card[] hand = new Card[7] { c1, c2, c3, c4, c5, c6, c7 };
            Card[] hand2 = new Card[7] { c8, c9, c10, c11, c12, c13, c14 };
            foreach (Card card in hand)
            {
                Console.Write("|" + card);
            }
            Console.WriteLine("|");
            foreach (Card card in hand2)
            {
                Console.Write("|" + card);
            }
            Console.WriteLine("|");

            HandEvaluator eval = new HandEvaluator();
            Console.WriteLine(eval.GetHandValue(hand));
            Console.WriteLine(eval.GetHandValue(hand2));
            Console.ReadLine();
        }
    }
}
