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

            Game game = new Game(players, 5, 10);
            game.StartGame();
        }
    }
}
