using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PioHoldem
{
    class ShowdownEvaluator
    {
        public ShowdownEvaluator()
        {

        }

        // Evaluate hole cards of the given players in combination with the 
        // board and return the index of the player with the best hand
        public int EvaluateHands(Player[] players, Card[] board)
        {
            int handRank;
            int[] handRanks = new int[players.Length];
            Card[] hand = new Card[7];

            // Populate the first five cards with the board
            for (int i = 0; i < 5; i++)
            {
                hand[i] = board[i];
            }

            for (int i = 0; i < players.Length; i++)
            {
                Player player = players[i];

                // Populate the last two cards with the current player's hole cards
                hand[5] = player.holeCards[0];
                hand[6] = player.holeCards[1];

                string output = player.name + " shows |" + player.holeCards[0] + "|" + player.holeCards[1] + "| ";

                // Find the rank of the current player's hand
                #region
                if (HasStraightFlush(hand))
                {
                    handRank = 8;
                    output += "*Straight Flush*";
                }
                else if (HasFourOfAKind(hand))
                {
                    handRank = 7;
                    output += "*Four Of A Kind*";
                }
                else if (HasFullHouse(hand))
                {
                    handRank = 6;
                    output += "*Full House*";
                }
                else if (HasFlush(hand))
                {
                    handRank = 5;
                    output += "*Flush*";
                }
                else if (HasStraight(hand))
                {
                    handRank = 4;
                    output += "*Straight*";
                }
                else if (HasThreeOfAKind(hand))
                {
                    handRank = 3;
                    output += "*Three Of A Kind*";
                }
                else if (HasTwoPair(hand))
                {
                    handRank = 2;
                    output += "*Two Pair*";
                }
                else if (HasPair(hand))
                {
                    handRank = 1;
                    output += "*Pair*";
                }
                else
                {
                    handRank = 0;
                    output += "*High Card*";
                }
                #endregion

                Console.WriteLine(output);

                // Write the rank to the list of hand ranks for each player
                handRanks[i] = handRank;
            }

            int highestRank = handRanks.Max();
            int winnerIndex = 0;

            int count = 0;
            for (int i = 0; i < handRanks.Length; i++)
            {
                if (handRanks[i] == highestRank)
                {
                    count++;
                    winnerIndex = i;
                }
            }

            // If there is more than one hand of the highest rank, we need to break the tie
            if (count > 1)
            {
                //return TieBreaker(int rank, Player[] players, Card[] board);
                return 0;
            }
            // Otherwise, return the index of the player with the highest ranking hand
            else
            {
                return winnerIndex;
            }
        }

        private bool HasStraightFlush(Card[] hand)
        {
            foreach (Card card in hand)
            {
                int suit = card.suit;
                int value = card.value;
                
                // Check for 5 cards of same suit and consecutive values
                if (HandContains(hand, suit, value + 1) && HandContains(hand, suit, value + 2) &&
                    HandContains(hand, suit, value + 3) && HandContains(hand, suit, value + 4))
                {
                    return true;
                }
                // Check for A2345 of same suit
                else if (value == 12)
                {
                    if (HandContains(hand, suit, 0) && HandContains(hand, suit, 1) &&
                        HandContains(hand, suit, 2) && HandContains(hand, suit, 3))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool HasFourOfAKind(Card[] hand)
        {
            foreach (Card card in hand)
            {
                int count = 0;
                foreach (Card card2 in hand)
                {
                    // Excluding the current card itself, count the cards in the hand that have the same value
                    if (card.suit != card2.suit && card.value == card2.value)
                    {
                        count++;
                        if (count == 3)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool HasFullHouse(Card[] hand)
        {
            int value1 = -1, value2 = -1;
            foreach (Card card in hand)
            {
                int count = 0;
                foreach (Card card2 in hand)
                {
                    // Check for 3 cards of the same value
                    if (card.suit != card2.suit && card.value == card2.value)
                    {
                        count++;
                        if (count == 2)
                        {
                            value1 = card.value;
                        }
                    }
                }
            }
            foreach (Card card in hand)
            {
                foreach (Card card2 in hand)
                {
                    // Check for 2 cards of the same value (different value than above)
                    if (card.suit != card2.suit && card.value == card2.value && card.value != value1)
                    {
                        value2 = card.value;
                    }
                }
            }
            if (value1 >= 0 && value2 >= 0)
            {
                return true;
            }
            return false;
        }

        private bool HasFlush(Card[] hand)
        {
            foreach (Card card in hand)
            {
                int count = 0;
                foreach (Card card2 in hand)
                {
                    // Excluding the current card itself, count the cards in the hand that have the same suit
                    if (card.value != card2.value && card.suit == card2.suit)
                    {
                        count++;
                        if (count == 4)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool HasStraight(Card[] hand)
        {
            foreach (Card card in hand)
            {
                int value = card.value;

                // Check for 5 cards of consecutive values
                if (HandContainsValueOnly(hand, value + 1) && HandContainsValueOnly(hand, value + 2) &&
                    HandContainsValueOnly(hand, value + 3) && HandContainsValueOnly(hand, value + 4))
                {
                    return true;
                }
                // Check for A2345
                else if (value == 12)
                {
                    if (HandContainsValueOnly(hand, 0) && HandContainsValueOnly(hand, 1) &&
                        HandContainsValueOnly(hand, 2) && HandContainsValueOnly(hand, 3))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool HasThreeOfAKind(Card[] hand)
        {
            foreach (Card card in hand)
            {
                int count = 0;
                foreach (Card card2 in hand)
                {
                    // Excluding the current card itself, count the cards in the hand that have the same value
                    if (card.suit != card2.suit && card.value == card2.value)
                    {
                        count++;
                        if (count == 2)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool HasTwoPair(Card[] hand)
        {
            int value1 = -1, value2 = -1;
            foreach (Card card in hand)
            {
                foreach (Card card2 in hand)
                {
                    // Check for 2 cards of the same value
                    if (card.suit != card2.suit && card.value == card2.value)
                    {
                        value1 = card.value;
                    }
                }
            }
            foreach (Card card in hand)
            {
                foreach (Card card2 in hand)
                {
                    // Check for 2 cards of the same value (different value than above)
                    if (card.suit != card2.suit && card.value == card2.value && card.value != value1)
                    {
                        value2 = card.value;
                    }
                }
            }
            if (value1 >= 0 && value2 >= 0)
            {
                return true;
            }
            return false;
        }

        private bool HasPair(Card[] hand)
        {
            foreach (Card card in hand)
            {
                foreach (Card card2 in hand)
                {
                    // Check for 2 cards of the same value
                    if (card.suit != card2.suit && card.value == card2.value)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Return true if the given hand contains a Card of the given suit and value
        private bool HandContains(Card[] hand, int suit, int value)
        {
            foreach (Card card in hand)
            {
                if (card.suit == suit && card.value == value)
                {
                    return true;
                }
            }
            return false;
        }

        // Return true if the given hand contains a Card of the given value
        private bool HandContainsValueOnly(Card[] hand, int value)
        {
            foreach (Card card in hand)
            {
                if (card.value == value)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
