using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PioHoldem
{
    class Game
    {
        public int pot, sbAmt, bbAmt, betAmt, btnIndex, sbIndex, bbIndex, actingIndex, actionCount;
        public bool isPreflop;
        public Player[] players;
        public Card[] board;
        private Deck deck;
        private Random rng;
        private HandEvaluator eval;

        public Game(Player[] players, int sbAmt, int bbAmt)
        {
            this.players = players;
            deck = new Deck();
            rng = new Random();
            eval = new HandEvaluator();
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

        // Main game flow logic
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
                isPreflop = true;
                bool allButOneFolded = false;
                allButOneFolded = BettingRound(GetNextPosition(bbIndex));
                if (!allButOneFolded)
                {
                    Flop();
                    isPreflop = false;
                    if (!AllInSkipToShowdown())
                    {
                        allButOneFolded = BettingRound(GetNextPosition(btnIndex));
                    }
                    if (!allButOneFolded)
                    {
                        Turn();
                        if (!AllInSkipToShowdown())
                        {
                            allButOneFolded = BettingRound(GetNextPosition(btnIndex));
                        }
                        if (!allButOneFolded)
                        {
                            River();
                            if (!AllInSkipToShowdown())
                            {
                                allButOneFolded = BettingRound(GetNextPosition(btnIndex));
                            }

                            if (!allButOneFolded)
                            {
                                Showdown();
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
                }
                else
                {
                    EndHand();
                }

                // Remove any players that busted during this hand
                players = RemoveBustedPlayers(players);

                // If there is only one non-folded player left, end the game
                if (players.Length == 1)
                {
                    gameOver = true;
                }
                // Otherwise, begin the next hand
                else
                {
                    Console.WriteLine();
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
            Console.WriteLine();
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
            actionCount = 0;
            actingIndex = toActFirstIndex;
            int toActLastIndex = GetToActLastIndex(toActFirstIndex);

            while (!settled)
            {
                // Get an action from the current player if they have not folded and are not all in
                if (!players[actingIndex].folded && players[actingIndex].stack > 0)
                {
                    // Print player stacks and other relevant amounts
                    PrintPlayers();
                    Console.WriteLine("Pot:" + pot + " Bet:" + betAmt + " InFor:" + players[actingIndex].inFor + " ToCall:" + (betAmt - players[actingIndex].inFor));

                    // Get an action from the acting player
                    int playerAction = players[actingIndex].GetAction(this);

                    // Based on the player's action, update applicable amounts
                    // and determine if the action in this betting round is settled
                    settled = ProcessPlayerAction(playerAction, toActLastIndex);

                    actionCount++;
                }
                
                // If all but one player have folded, end the hand
                if (CountNotFolded() == 1)
                {
                    return true;
                }

                // Get the next player to act
                actingIndex = GetNextPosition(actingIndex);
            }
            return false;
        }

        // Receive the action of the player to act and update applicable amounts
        // Return true if action is settled in this betting round for all players, false if not
        private bool ProcessPlayerAction(int playerAction, int toActLastIndex)
        {
            // Negative value: the player folded
            if (playerAction < 0)
            {
                Console.WriteLine();
                Console.WriteLine(players[actingIndex].name + " folds");
                Console.WriteLine();
                players[actingIndex].folded = true;
            }
            // Value of 0: the player checked
            else if (playerAction == 0)
            {
                Console.WriteLine();
                Console.WriteLine(players[actingIndex].name + " checks");
                Console.WriteLine();
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
                Console.WriteLine();
                if (betAmt > 0 && playerAction + players[actingIndex].inFor <= betAmt)
                {
                    string toPrint = players[actingIndex].name + " calls " + playerAction;
                    if (playerAction < betAmt && playerAction != playerAction + players[actingIndex].inFor)
                        toPrint += " (" + (playerAction + players[actingIndex].inFor) + " total)";
                    Console.Write(toPrint);
                }
                else if (betAmt == 0)
                {
                    Console.Write(players[actingIndex].name + " bets " + playerAction);
                }
                else
                {
                    Console.Write(players[actingIndex].name + " raises to " + (playerAction + players[actingIndex].inFor));
                }
                
                Console.WriteLine(playerAction >= players[actingIndex].stack ? " *ALL IN*" : "");
                Console.WriteLine();

                // If a player calls all in for less than the bet amount, return the difference to the other player
                // *** ONLY WORKS FOR HEADS-UP ***
                if (playerAction + players[actingIndex].inFor < betAmt)
                {
                    int diff = betAmt - (playerAction + players[actingIndex].inFor);
                    Console.WriteLine(players[actingIndex].name + " is covered - returning difference of " + diff + 
                        " to " + players[GetNextPosition(actingIndex)].name);
                    Console.WriteLine();
                    pot -= diff;
                    players[GetNextPosition(actingIndex)].stack += diff;
                    players[GetNextPosition(actingIndex)].inFor -= diff;
                    betAmt -= diff;
                }
                
                // Update applicable amounts
                pot += playerAction;
                players[actingIndex].stack -= playerAction;
                players[actingIndex].inFor += playerAction;
                betAmt = players[actingIndex].inFor;
            }

            // Close the betting action on this betting round unless...
            bool isActionClosed = true;

            // ...at least one non-folded player has not matched the betAmt
            foreach (Player player in players)
            {
                if (!player.folded && player.inFor != betAmt)
                {
                    isActionClosed = false;
                }
            }

            // ...the player in the big blind is next to act and no player has raised
            if (bbIndex == GetNextPosition(actingIndex) && betAmt == bbAmt && isPreflop)
            {
                isActionClosed = false;
            }

            return isActionClosed;
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
            Thread.Sleep(1000);
            deck.Burn();
            board[0] = deck.Deal();
            board[1] = deck.Deal();
            board[2] = deck.Deal();
            Console.Write("Flop:  ");
            PrintBoard();
            Console.WriteLine();
            ClearInFor();
        }

        // Deal the Turn
        private void Turn()
        {
            Thread.Sleep(1000);
            deck.Burn();
            board[3] = deck.Deal();
            Console.Write("Turn:  ");
            PrintBoard();
            Console.WriteLine();
            ClearInFor();
        }

        // Deal the River
        private void River()
        {
            Thread.Sleep(1000);
            deck.Burn();
            board[4] = deck.Deal();
            Console.Write("River: ");
            PrintBoard();
            Console.WriteLine();
            ClearInFor();
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
            Thread.Sleep(1000);
            Console.WriteLine("Showdown!");
            Console.Write("Board: ");
            PrintBoard();
            int showdownCount = CountNotFolded();
            Player[] showdownPlayers = new Player[showdownCount];

            int trackerIndex = 0;
            foreach (Player player in players)
            {
                if (!player.folded)
                {
                    showdownPlayers[trackerIndex] = player;
                    trackerIndex++;
                }
            }

            int winnerIndex = eval.EvaluateHands(showdownPlayers, board);
            if (winnerIndex < 0)
            {
                Console.WriteLine("Chop pot!");
                int chopAmt = pot / showdownCount;
                int remainder = pot % showdownCount;
                int remainderIndex = GetToActLastIndex(sbIndex);
                for (int i = 0; i < players.Length; i++)
                {
                    if (!players[i].folded)
                    {
                        int take = chopAmt + (i == remainderIndex ? remainder : 0);
                        Console.WriteLine(players[i].name + " wins " + take);
                        players[i].stack += take;
                    }
                }
            }
            else
            {
                Console.WriteLine(players[winnerIndex].name + " wins pot of " + pot);
                players[winnerIndex].stack += pot;
            }

            // Move the button for the start of the next hand
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

        private void PrintStacksAndPot()
        {
            PrintPlayers();
            Console.WriteLine("Pot:" + pot);
        }

        // Count the number of players that have not folded
        private int CountNotFolded()
        {
            int playersRemaining = 0;
            foreach (Player player in players)
            {
                if (!player.folded)
                {
                    playersRemaining++;
                }
            }
            return playersRemaining;
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
        public int GetNextPosition(int index)
        {
            return index + 1 == players.Length ? 0 : index + 1;
        }

        // Get the index of the previous player to act
        public int GetPreviousPosition(int index)
        {
            return index - 1 == -1 ? players.Length - 1 : index - 1;
        }

        // Skip to showdown if all but one (or all) players are all in
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

        // Return a list of players that have a chip stack > 0
        public Player[] RemoveBustedPlayers(Player[] players)
        {
            int bustedCount = 0;
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].stack == 0)
                {
                    Console.WriteLine(players[i].name + " busted");
                    players[i] = null;
                    bustedCount++;
                }
            }

            Player[] remainingPlayers = new Player[players.Length - bustedCount];
            int trackerIndex = 0;
            foreach (Player player in players)
            {
                if (player != null)
                {
                    remainingPlayers[trackerIndex] = player;
                    trackerIndex++;
                }
            }

            return remainingPlayers;
        }

        // End the game and announce the winner
        private void GameOver()
        {
            Console.WriteLine(players[0].name + " wins!");
            Console.WriteLine();
            Console.WriteLine("Press ENTER to end game...");
            Console.ReadLine();
        }
    }
}
