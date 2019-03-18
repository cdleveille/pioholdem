using System;

namespace PioHoldem
{
    class Client
    {
        static void Main(string[] args)
        {
            Player p1 = new HumanPlayer("Chris", 200);
            Player p2 = new HumanPlayer("Aaron", 200);
            //Player p2 = new BotPlayer("Aaron", 200);

            Player[] players = new Player[2];
            players[0] = p1;
            players[1] = p2;
            //players[2] = p3;

            Game game = new Game(players, 5, 10);
            game.StartGame();
        }
    }
}
