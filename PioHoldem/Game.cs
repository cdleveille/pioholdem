using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PioHoldem
{
    class Game
    {
        public int pot, sbAmt, bbAmt, currentBetAmt, btnIndex, sbIndex, bbIndex, toActIndex;
        public Player[] players;
        public Deck deck;
        public Card[] board;
        private Random rng;

        public Game(Player[] players, int sbAmt, int bbAmt)
        {
            this.players = players;
            deck = new Deck();
            rng = new Random();
            this.sbAmt = sbAmt;
            this.bbAmt = bbAmt;
            board = new Card[5];
        }

        // Start a new game
        public void StartGame()
        {
            GameLoop();
        }

        // Main flow of the game
        public void GameLoop()
        {
            btnIndex = rng.Next(players.Length);
            bool gameOver = false;
            while (!gameOver)
            {
                bool handOver = false;
                Console.WriteLine("*****NEW HAND*****");
                pot = 0;
                Console.Write(players.Length + " players: ");
                PrintPlayers();
                Console.WriteLine(players[btnIndex].name + " is on the button.");
                deck.Shuffle();
                DealHoleCards();
                PostBlinds();
                Console.WriteLine("Pot: " + pot);
                handOver = BettingRound(GetNextPosition(bbIndex));

                if (!handOver)
                {
                    //Flop();
                    // ...
                }
                else
                {
                    // EndHand();
                }

                Console.ReadLine();

            }
        }

        // Collect the blinds
        private void PostBlinds()
        {

            // If we are heads-up, the button posts the small blind
            if (players.Length == 2)
            {
                sbIndex = btnIndex;
                bbIndex = GetNextPosition(sbIndex);
            }
            else
            {
                sbIndex = GetNextPosition(btnIndex);
                bbIndex = GetNextPosition(sbIndex);
            }
            
            if (players[sbIndex].stack > sbAmt)
            {
                pot += sbAmt;
                players[sbIndex].inForOnCurrentStreet += sbAmt;
                Console.WriteLine(players[sbIndex].name + " posts the small blind of " + sbAmt + ".");
                players[sbIndex].decStack(sbAmt);
            }
            else
            {
                pot += players[sbIndex].stack;
                players[sbIndex].inForOnCurrentStreet += players[sbIndex].stack;
                Console.WriteLine(players[sbIndex].name + " posts the small blind of " + players[sbIndex].stack + " (ALL IN).");
                players[sbIndex].decStack(sbAmt);
            }

            if (players[bbIndex].stack > bbAmt)
            {
                pot += bbAmt;
                players[bbIndex].inForOnCurrentStreet += bbAmt;
                currentBetAmt = bbAmt;
                Console.WriteLine(players[bbIndex].name + " posts the big blind of " + bbAmt + ".");
                players[bbIndex].decStack(bbAmt);
            }
            else
            {
                pot += players[bbIndex].stack;
                players[bbIndex].inForOnCurrentStreet += players[bbIndex].stack;
                currentBetAmt = players[bbIndex].stack;
                Console.WriteLine(players[bbIndex].name + " posts the big blind of " + players[bbIndex].stack + " (ALL IN).");
                players[bbIndex].decStack(bbAmt);
            }
        }

        // Deal two hole cards to each player
        private void DealHoleCards()
        {
            foreach (Player player in players)
            {
                player.holeCards = deck.Deal(2);
                player.folded = false;
                player.inForOnCurrentStreet = 0;
            }
        }

        // Get an action from each player in order until all players 
        // have either folded or committed the required amount of chips
        private bool BettingRound(int toActFirstIndex)
        {
            bool settled = false;
            toActIndex = toActFirstIndex;
            while (!settled)
            {
                // Get an action from the current player if they have not folded and are not all in
                if (!players[toActIndex].folded && players[toActIndex].stack > 0)
                {
                    settled = ProcessPlayerAction(players[toActIndex].GetAction(this));
                }

                // Count the number of players that have not folded
                int playersRemaining = 0;
                foreach (Player player in players)
                {
                    if (!player.folded)
                    {
                        playersRemaining++;
                    }
                }

                // If all but one player have folded, end the hand
                if (playersRemaining == 1)
                {
                    return true;
                }

                // Get the next player to act
                toActIndex = GetNextPosition(toActIndex);
            }
            return false;
        }

        // Get the index of the next player to act
        private int GetNextPosition(int index)
        {
            return index + 1 == players.Length ? 0 : index + 1;
        }

        // Receive the action of the player to act and update necessary game objects
        // Return true if action is settled on this street for all players, false if not
        private bool ProcessPlayerAction(int playerAction)
        {
            // Negative value: the player folded
            if (playerAction < 0)
            {
                players[toActIndex].folded = true;
            }
            // If the big blind checks the option, close the action
            else if (playerAction == 0)
            {
                if (toActIndex == bbIndex && currentBetAmt == bbAmt)
                {
                    return true;
                }
            }
            // Positive value: the player put chips in the pot (bet/call/raise)
            else if (playerAction > 0)
            {
                players[toActIndex].inForOnCurrentStreet += playerAction;
                currentBetAmt = players[toActIndex].inForOnCurrentStreet;
                pot += playerAction;
            }

            bool toReturn = true;
            foreach (Player player in players)
            {
                // If a player that has not folded has not matched the currentBetAmt, do not close the action
                if (!player.folded && player.inForOnCurrentStreet != currentBetAmt)
                {
                    toReturn = false;
                }

                // If the big blind is next to act and no one has raised, do not close the action
                if (bbIndex == GetNextPosition(toActIndex) && currentBetAmt == bbAmt)
                {
                    toReturn = false;
                }
            }

            return toReturn;
        }

        private void PrintPlayers()
        {
            foreach (Player player in players)
            {
                Console.Write(player.ToString() + " ");
            }
            Console.WriteLine();
        }
    }
}
