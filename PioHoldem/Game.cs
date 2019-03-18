using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PioHoldem
{
    class Game
    {
        public int pot, sbAmt, bbAmt, betAmt, btnIndex, sbIndex, bbIndex, actingIndex;
        public Player[] players;
        public Card[] board;
        private Deck deck;
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
            btnIndex = rng.Next(players.Length);
            GameLoop();
        }

        // Main flow of the game
        private void GameLoop()
        {
            bool gameOver = false;
            while (!gameOver)
            {
                Console.Clear();
                Console.WriteLine("************NEW HAND************");
                pot = 0;
                ClearBoard();
                Console.Write(players.Length + " players: ");
                PrintPlayers();
                Console.WriteLine(players[btnIndex].name + " is on the button");
                deck.Shuffle();
                DealHoleCards();
                PostBlinds();

                bool allButOneFolded = false;
                allButOneFolded = BettingRound(GetNextPosition(bbIndex));
                if (!allButOneFolded)
                {
                    Flop();
                    ClearInFor();
                    if (!AllInSkipToShowdown())
                    {
                        allButOneFolded = BettingRound(GetNextPosition(btnIndex));
                    }
                    if (!allButOneFolded)
                    {
                        Turn();
                        ClearInFor();
                        if (!AllInSkipToShowdown())
                        {
                            allButOneFolded = BettingRound(GetNextPosition(btnIndex));
                        }
                        if (!allButOneFolded)
                        {
                            River();
                            ClearInFor();
                            if (!AllInSkipToShowdown())
                            {
                                allButOneFolded = BettingRound(GetNextPosition(btnIndex));
                            }
                            if (!allButOneFolded)
                            {
                                Showdown();
                            }
                        }
                        else
                        {
                            EndHand();
                        }
                    }
                    else
                    {
                        EndHand();
                    }
                }
                else
                {
                    EndHand();
                }

                // Exit game loop if a player busted
                if (PlayerBusted())
                {
                    gameOver = true;
                }
                // Otherwise, continue the game
                else
                {
                    Console.WriteLine("Press ENTER to begin next hand...");
                    Console.ReadLine();
                }
            }
            GameOver();
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
                players[sbIndex].inFor += sbAmt;
                Console.WriteLine(players[sbIndex].name + " posts the small blind of " + sbAmt);
                players[sbIndex].stack -= sbAmt;
            }
            else
            {
                pot += players[sbIndex].stack;
                players[sbIndex].inFor += players[sbIndex].stack;
                Console.WriteLine(players[sbIndex].name + " posts the small blind of " + players[sbIndex].stack + " (ALL IN)");
                players[sbIndex].stack = 0;
            }

            if (players[bbIndex].stack > bbAmt)
            {
                pot += bbAmt;
                players[bbIndex].inFor += bbAmt;
                betAmt = bbAmt;
                Console.WriteLine(players[bbIndex].name + " posts the big blind of " + bbAmt);
                players[bbIndex].stack -= bbAmt;
            }
            else
            {
                pot += players[bbIndex].stack;
                players[bbIndex].inFor += players[bbIndex].stack;
                betAmt = players[bbIndex].stack;
                Console.WriteLine(players[bbIndex].name + " posts the big blind of " + players[bbIndex].stack + " (ALL IN)");
                players[bbIndex].stack = 0;
            }
        }

        // Deal two hole cards to each player
        private void DealHoleCards()
        {
            foreach (Player player in players)
            {
                player.holeCards = deck.Deal(2);
                player.folded = false;
                player.inFor = 0;
            }
        }

        // Get an action from each player in order until all players 
        // have either folded or committed the required amount of chips
        private bool BettingRound(int toActFirstIndex)
        {
            bool settled = false;
            actingIndex = toActFirstIndex;
            int toActLastIndex = GetToActLastIndex(toActFirstIndex);

            while (!settled)
            {
                // Get an action from the current player if they have not folded and are not all in
                if (!players[actingIndex].folded && players[actingIndex].stack > 0)
                {
                    PrintPlayers();
                    Console.WriteLine("Pot:" + pot + " Bet:" + betAmt + " InFor:" + players[actingIndex].inFor + " ToCall:" + (betAmt - players[actingIndex].inFor));

                    settled = ProcessPlayerAction(players[actingIndex].GetAction(this), toActLastIndex);
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
                actingIndex = GetNextPosition(actingIndex);
            }
            return false;
        }

        // Receive the action of the player to act and update necessary game objects
        // Return true if action is settled on this street for all players, false if not
        private bool ProcessPlayerAction(int playerAction, int toActLastIndex)
        {
            // Negative value: the player folded
            if (playerAction < 0)
            {
                Console.WriteLine(players[actingIndex].name + " folds");
                players[actingIndex].folded = true;
            }
            // Value of 0: the player checked
            else if (playerAction == 0)
            {
                Console.WriteLine(players[actingIndex].name + " checks");
                // Close the action if the big blind checks the option
                if (actingIndex == bbIndex && betAmt == bbAmt)
                {
                    return true;
                }
                // Leave the action open if there is no bet and not all players have acted yet
                else if (betAmt == 0 && actingIndex != toActLastIndex)
                {
                    return false;
                }
            }
            // Positive value: the player put chips in the pot (call/bet/raise)
            else if (playerAction > 0)
            {
                if (betAmt > 0 && playerAction + players[actingIndex].inFor <= betAmt)
                {
                    Console.Write(players[actingIndex].name + " calls " + (playerAction + players[actingIndex].inFor));
                }
                else if (betAmt == 0)
                {
                    Console.Write(players[actingIndex].name + " bets " + playerAction);
                }
                else
                {
                    Console.Write(players[actingIndex].name + " raises to " + (playerAction + players[actingIndex].inFor));
                }
                
                Console.WriteLine(playerAction >= players[actingIndex].stack ? " (ALL IN)" : "");

                // If a player calls all in for less than the bet amount, return the difference to the other player
                // *** ONLY WORKS FOR HEADS-UP ***
                if (playerAction + players[actingIndex].inFor < betAmt)
                {
                    int diff = betAmt - (playerAction + players[actingIndex].inFor);
                    Console.WriteLine("Returning difference of " + diff + " to " + players[GetNextPosition(actingIndex)].name);
                    pot -= diff;
                    players[GetNextPosition(actingIndex)].stack += diff;
                    players[GetNextPosition(actingIndex)].inFor -= diff;
                    betAmt -= diff;
                }
                
                players[actingIndex].inFor += playerAction;
                players[actingIndex].stack -= playerAction;
                betAmt = players[actingIndex].inFor;
                pot += playerAction;
            }

            // Close the action on this street unless...
            bool toReturn = true;

            // ...at least one player that has not folded has not matched the betAmt
            foreach (Player player in players)
            {
                if (!player.folded && player.inFor != betAmt)
                {
                    toReturn = false;
                }
            }

            // ...the player in the big blind is next to act and no player has raised
            if (bbIndex == GetNextPosition(actingIndex) && betAmt == bbAmt)
            {
                toReturn = false;
            }

            return toReturn;
        }

        // Print a list of each player and their stack size
        private void PrintPlayers()
        {
            foreach (Player player in players)
            {
                Console.Write(player + " ");
            }
            Console.WriteLine();
        }

        // Deal the Flop
        private void Flop()
        {
            deck.Burn();
            board[0] = deck.Deal();
            board[1] = deck.Deal();
            board[2] = deck.Deal();
            Console.Write("Flop:  ");
            PrintBoard();
        }

        // Deal the Turn
        private void Turn()
        {
            deck.Burn();
            board[3] = deck.Deal();
            Console.Write("Turn:  ");
            PrintBoard();
        }

        // Deal the River
        private void River()
        {
            deck.Burn();
            board[4] = deck.Deal();
            Console.Write("River: ");
            PrintBoard();
        }

        // Award to the pot to the only remaining player if all others have folded
        private void EndHand()
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].folded == false)
                {
                    Console.WriteLine(players[i].name + " wins pot of " + pot);
                    players[i].stack += pot;
                }
            }
            btnIndex = GetNextPosition(btnIndex);
        }

        // Reveal hole cards of remaining players and award the pot to the winner
        private void Showdown()
        {
            Console.WriteLine("Showdown!");
            foreach (Player player in players)
            {
                if (!player.folded)
                {
                    Console.WriteLine(player.name + " shows |" + player.holeCards[0] + "|" + player.holeCards[1] + "|");
                }
            }

            Console.WriteLine("HandEvaluator not yet implemented! Assigning random winner...");

            int winner = rng.Next(players.Length);
            Console.WriteLine(players[winner].name + " wins pot of " + pot);
            players[winner].stack += pot;
            btnIndex = GetNextPosition(btnIndex);
        }

        // Set inFor to 0 for all players
        private void ClearInFor()
        {
            betAmt = 0;
            foreach (Player player in players)
            {
                player.inFor = 0;
            }
        }
        
        // Return the index of the last player to act on the current street
        private int GetToActLastIndex(int toActFirstIndex)
        {
            if (!players[GetPreviousPosition(toActFirstIndex)].folded)
            {
                return GetPreviousPosition(toActFirstIndex);
            }
            else
            {
                return GetToActLastIndex(GetPreviousPosition(toActFirstIndex));
            }
        }

        // Print the community cards
        private void PrintBoard()
        {
            foreach (Card card in board)
            {
                if (card != null)
                {
                    Console.Write("|" + card);
                }
                else
                {
                    Console.Write("|  ");
                }
            }
            Console.WriteLine("|");
        }

        // Reset the community cards
        private void ClearBoard()
        {
            for (int i = 0; i < board.Length; i++)
            {
                board[i] = null;
            }
        }

        // Get the index of the next player to act
        private int GetNextPosition(int index)
        {
            return index + 1 == players.Length ? 0 : index + 1;
        }

        // Get the index of the previous player to act
        private int GetPreviousPosition(int index)
        {
            return index - 1 == -1 ? players.Length - 1 : index - 1;
        }

        // Skip to showdown if enough players are all in
        private bool AllInSkipToShowdown()
        {
            int notFoldedCount = 0;
            int allInCount = 0;
            foreach (Player player in players)
            {
                if (!player.folded)
                {
                    notFoldedCount++;
                    if (player.stack == 0)
                    {
                        allInCount++;
                    }
                }
            }
            if (notFoldedCount - allInCount <= 1)
            {
                return true;
            }
            return false;
        }

        // Return true if a player busted during the past hand
        private bool PlayerBusted()
        {
            foreach (Player player in players)
            {
                if (player.stack == 0)
                {

                    return true;
                }
            }
            return false;
        }

        // End the game and announce the winner
        // *** ONLY WORKS FOR HEADS-UP ***
        private void GameOver()
        {
            foreach (Player player in players)
            {
                if (player.stack == 0)
                {
                    Console.WriteLine(player.name + " busted");
                }
            }
            foreach (Player player in players)
            {
                if (player.stack > 0)
                {
                    Console.WriteLine(player.name + " wins!");
                }
            }

            Console.WriteLine("Press ENTER to end game...");
            Console.ReadLine();
        }
    }
}
