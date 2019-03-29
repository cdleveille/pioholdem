using System;
using System.Threading;

namespace PioHoldem
{
    class Game
    {
        public int pot, sbAmt, bbAmt, betAmt, prevBetAmount, btnIndex, 
            sbIndex, bbIndex, actingIndex, street, actionCount, sleepTime;
        public double effectiveStack;
        public Player[] players;
        public Card[] board;
        private Deck deck;
        private Random rng;
        private HandEvaluator eval;

        // Create a new game with the specified players, stakes, and delay time between actions
        public Game(Player[] players, int sbAmt, int bbAmt, int sleepTime)
        {
            this.players = players;
            deck = new Deck();
            rng = new Random();
            eval = new HandEvaluator();
            this.sbAmt = sbAmt;
            this.bbAmt = bbAmt;
            this.sleepTime = sleepTime;
            board = new Card[5];
        }

        // Start a new game
        public void StartGame()
        {
            foreach (Player player in players)
            {
                player.busted = false;
                player.stack = player.buyinAmt;
            }
            btnIndex = rng.Next(players.Length);
            pot = 0;
            GameLoop();
        }

        // Main game flow logic
        private void GameLoop()
        {
            bool gameOver = false, allButOneFolded;
            int handCount = 1;
            while (!gameOver)
            {
                Preflop();

                allButOneFolded = BettingRound(GetNextPosition(bbIndex));
                if (!allButOneFolded)
                {
                    Flop();
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

                // If there is only one remaining player with a chip stack, end the game
                if (CountNotBusted() == 1)
                {
                    gameOver = true;
                }
                // Otherwise, begin the next hand
                else
                {
                    handCount++;
                    Console.WriteLine();
                    Console.WriteLine("Press ENTER to begin next hand...");
                    Console.ReadLine();
                }
            }
            GameOver();
        }

        // Show output and take necessary actions at the start of a new hand
        private void Preflop()
        {
            Console.Clear();
            PrintScore();
            Console.WriteLine("************NEW HAND************");
            Console.Write(players.Length + " players: ");
            PrintPlayers();
            Console.WriteLine(players[btnIndex].name + " is on the button");

            ClearBoard();
            ClearBetAmts();
            effectiveStack = CalculateEffectiveStack();

            PostBlinds();
            deck.Shuffle();
            DealHoleCards();

            street = 0;
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
                Console.WriteLine(players[sbIndex].name + " posts the small blind of " + players[sbIndex].stack + " *ALL IN*");
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
                Console.WriteLine(players[bbIndex].name + " posts the big blind of " + players[bbIndex].stack + " *ALL IN*");
                players[bbIndex].stack = 0;
            }
            Console.WriteLine();
        }

        // Deal two hole cards to each player
        private void DealHoleCards()
        {
            foreach (Player player in players)
            {
                if (!player.busted)
                {
                    player.holeCards = deck.Deal(2);
                    player.folded = false;
                    player.isAggressor = false;
                }
            }
        }

        // Get an action from each player in order until all players have either folded or 
        // committed the required amount of chips. Return true if all but one player have 
        // folded, or false if the hand should continue.
        private bool BettingRound(int toActFirstIndex)
        {
            bool settled = false;
            actionCount = 0;
            actingIndex = toActFirstIndex;
            int toActLastIndex = GetToActLastIndex(toActFirstIndex);

            while (!settled)
            {
                // Get an action from the current player if they have not folded and are not all in
                if (!players[actingIndex].folded && players[actingIndex].stack > 0 && !players[actingIndex].busted)
                {
                    // Print player stacks and other relevant amounts
                    PrintPlayers();
                    Console.Write("Pot:" + pot + " Bet:" + betAmt + " InFor:" + players[actingIndex].inFor + " ToCall:");
                    if (betAmt - players[actingIndex].inFor >= players[actingIndex].stack)
                    {
                        Console.WriteLine("ALL-IN");
                    }
                    else
                    {
                        Console.WriteLine(betAmt - players[actingIndex].inFor);
                    }

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

        // Handle the action of the acting player and update applicable amounts.
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
                players[actingIndex].isAggressor = false;
            }
            // Value of 0: the player checked
            else if (playerAction == 0)
            {
                Console.WriteLine();
                Console.WriteLine(players[actingIndex].name + " checks");
                Console.WriteLine();
                players[actingIndex].isAggressor = false;

                // Close the action if the big blind checks and there has been no aggressive action
                if (actingIndex == bbIndex && (betAmt == bbAmt || betAmt == 0))
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
                    if (playerAction < betAmt && players[actingIndex].inFor > 0)
                        toPrint += " (" + (playerAction + players[actingIndex].inFor) + " total)";
                    Console.Write(toPrint);
                }
                else if (betAmt == 0)
                {
                    Console.Write(players[actingIndex].name + " bets " + playerAction);
                    players[actingIndex].isAggressor = true;
                    players[GetNextPosition(actingIndex)].isAggressor = false;
                }
                else
                {
                    Console.Write(players[actingIndex].name + " raises to " + (playerAction + players[actingIndex].inFor));
                    players[actingIndex].isAggressor = true;
                    players[GetNextPosition(actingIndex)].isAggressor = false;
                }
                
                Console.WriteLine(playerAction >= players[actingIndex].stack ? " *ALL IN*" : "");
                Console.WriteLine();

                // If a player calls all in for less than the bet amount, return the difference to the other player
                // *** ONLY WORKS FOR HEADS-UP ***
                if (playerAction + players[actingIndex].inFor < betAmt)
                {
                    int diff = betAmt - (playerAction + players[actingIndex].inFor);
                    Console.WriteLine(players[actingIndex].name + " is covered - returning " + diff + 
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
                prevBetAmount = betAmt;
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
            if (bbIndex == GetNextPosition(actingIndex) && betAmt == bbAmt && street == 0 && players[bbIndex].stack > 0)
            {
                isActionClosed = false;
            }

            return isActionClosed;
        }

        // Deal the Flop
        private void Flop()
        {
            street = 1;
            ClearBetAmts();
            deck.Burn();
            board[0] = deck.Deal();
            board[1] = deck.Deal();
            board[2] = deck.Deal();

            Thread.Sleep(sleepTime);
            Console.Write("Flop:  ");
            PrintBoard();
            Console.WriteLine();
        }

        // Deal the Turn
        private void Turn()
        {
            street = 2;
            ClearBetAmts();
            deck.Burn();
            board[3] = deck.Deal();

            Thread.Sleep(sleepTime);
            Console.Write("Turn:  ");
            PrintBoard();
            Console.WriteLine();
        }

        // Deal the River
        private void River()
        {
            street = 3;
            ClearBetAmts();
            deck.Burn();
            board[4] = deck.Deal();

            Thread.Sleep(sleepTime);
            Console.Write("River: ");
            PrintBoard();
            Console.WriteLine();
        }

        // Award to the pot to the only remaining player if all others have folded
        private void EndHand()
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].folded == false)
                {
                    Console.WriteLine(players[i].name + " wins pot of " + (pot - players[i].inFor));
                    players[i].stack += pot;
                    pot = 0;
                }
            }

            // Move the button for the start of the next hand
            btnIndex = GetNextPosition(btnIndex);
        }

        // Reveal hole cards of remaining players and award the pot to the winner(s)
        // *** ONLY WORKS FOR HEADS-UP ***
        private void Showdown()
        {
            Thread.Sleep(sleepTime);
            Console.WriteLine("Showdown!");
            Console.Write("Board: ");
            PrintBoard();
            int showdownCount = CountNotFolded(), trackerIndex = 0;
            Player[] showdownPlayers = new Player[showdownCount];


            foreach (Player player in players)
            {
                if (!player.folded)
                {
                    showdownPlayers[trackerIndex] = player;
                    trackerIndex++;
                }
            }

            int winnerIndex = eval.EvaluateHands(showdownPlayers, board);
            Console.WriteLine();
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
            pot = 0;

            // Report any players that busted during this hand
            ReportBustedPlayers(players);

            // Move the button for the start of the next hand
            btnIndex = GetNextPosition(btnIndex);
        }

        // Reset the committed number of chips to 0 for each player
        private void ClearBetAmts()
        {
            betAmt = 0;
            prevBetAmount = 0;
            foreach (Player player in players)
            {
                player.inFor = 0;
            }
        }

        // Reset the community cards
        private void ClearBoard()
        {
            for (int i = 0; i < board.Length; i++)
            {
                board[i] = null;
            }
        }

        // Print the community cards
        private void PrintBoard()
        {
            foreach (Card card in board)
            {
                Console.Write(card != null ? "|" + card : "|  ");
            }
            Console.WriteLine("|");
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

        // Show the number of games that each player has won
        private void PrintScore()
        {
            foreach (Player player in players)
            {
                Console.Write(player.name + ":" + player.winCount + " ");
            }
            Console.WriteLine();
        }

        // Get the index of the next player to act
        public int GetNextPosition(int index)
        {
            return index + 1 >= players.Length ? 0 : index + 1;
        }

        // Get the index of the previous player to act
        public int GetPreviousPosition(int index)
        {
            return index - 1 <= -1 ? players.Length - 1 : index - 1;
        }

        // Get the index of the last player to act on the current street
        private int GetToActLastIndex(int toActFirstIndex)
        {
            // The player in the BB always acts last preflop
            if (street == 0)
            {
                return bbIndex;
            }
            // Postflop, find the non-folded player in latest position
            else
            {
                return players[GetPreviousPosition(toActFirstIndex)].folded ? 
                    GetToActLastIndex(GetPreviousPosition(toActFirstIndex)) : GetPreviousPosition(toActFirstIndex);
            }
        }

        // Divide the shortest stack by the BB amount
        private double CalculateEffectiveStack()
        {
            int shortestStack = players[0].stack;
            foreach (Player player in players)
            {
                if (player.stack < shortestStack)
                {
                    shortestStack = player.stack;
                }
            }
            return (double)shortestStack / bbAmt;
        }

        // Skip to showdown if all but one (or all) non-folded players are all in
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

        // Count the number of players that have not busted
        private int CountNotBusted()
        {
            int count = 0;
            foreach (Player player in players)
            {
                if (!player.busted)
                {
                    count++;
                }
            }
            return count;
        }

        // Report any players that busted during this hand
        private void ReportBustedPlayers(Player[] players)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].stack == 0 && !players[i].busted)
                {
                    Console.WriteLine();
                    Console.WriteLine(players[i].name + " busted");
                    players[i].busted = true;
                }
            }
        }

        // End the game and announce the winner
        private void GameOver()
        {
            string winner = "";
            foreach (Player player in players)
            {
                if (!player.busted)
                {
                    winner = player.name;
                    player.winCount++;
                }
            }

            Console.WriteLine(winner + " wins!");
            Console.WriteLine();
            Console.WriteLine("Press ENTER to start new game...");
            Console.ReadLine();
        }
    }
}
