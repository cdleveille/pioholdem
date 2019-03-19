using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PioHoldem
{
    class HandEvaluator
    {
        public HandEvaluator()
        {

        }

        // Evaluate hole cards of the given players in combination with the 
        // board and return the index of the player with the best hand
        public int EvaluateHands(Player[] players, Card[] board)
        {
            int handValue, boardLength = 0;
            int[] handValues = new int[players.Length];

            foreach (Card card in board)
            {
                if (card != null)
                    boardLength++;
            }

            Card[] hand = new Card[2 + boardLength];

            // Populate the first five cards with the board
            for (int i = 0; i < boardLength; i++)
            {
                hand[i] = board[i];
            }

            for (int i = 0; i < players.Length; i++)
            {
                Player player = players[i];

                // Populate the last two cards with the current player's hole cards
                hand[boardLength] = player.holeCards[0];
                hand[boardLength + 1] = player.holeCards[1];

                Console.Write(player.name + " shows |" + player.holeCards[0] + "|" + player.holeCards[1] + "| ");

                // Calculate the relative value of the current player's hand
                handValue = GetHandValue(hand);

                // Write the value to the list of hand values for each player
                handValues[i] = handValue;
            }

            int highestHandValue = handValues.Max();
            int winnerIndex = 0;

            int count = 0;
            for (int i = 0; i < handValues.Length; i++)
            {
                if (handValues[i] == highestHandValue)
                {
                    count++;
                    winnerIndex = i;
                }
            }

            // If there is a tie, return -1 to indicate a chop pot
            if (count > 1)
            {
                return -1;
            }
            // Otherwise, return the index of the player with the highest ranking hand
            else
            {
                return winnerIndex;
            }
        }

        // Calculate the relative value of the given hand
        public int GetHandValue(Card[] hand)
        {
            // Start by checking for the highest ranking hand
            return HasStraightFlush(hand);
        }

        private int HasStraightFlush(Card[] hand)
        {
            foreach (Card card in hand)
            {
                int suit = card.suit;
                int value = card.value;
                
                // Check for 5 cards of same suit and consecutive values
                if (HandContains(hand, suit, value + 1) && HandContains(hand, suit, value + 2) &&
                    HandContains(hand, suit, value + 3) && HandContains(hand, suit, value + 4))
                {
                    Console.WriteLine("*Straight Flush*");
                    return 8000 + (value + 4);
                }
                // Check for A2345 of same suit
                else if (value == 12)
                {
                    if (HandContains(hand, suit, 0) && HandContains(hand, suit, 1) &&
                        HandContains(hand, suit, 2) && HandContains(hand, suit, 3))
                    {
                        Console.WriteLine("*Straight Flush*");
                        return 8003;
                    }
                }
            }
            return HasFourOfAKind(hand);
        }

        private int HasFourOfAKind(Card[] hand)
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
                            Console.WriteLine("*Four Of A Kind*");
                            return 7000 + (4 * card.value) + (SumOfCardValues(hand) - (4 * card.value));
                        }
                    }
                }
            }
            return HasFullHouse(hand);
        }

        private int HasFullHouse(Card[] hand)
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
                Console.WriteLine("*Full House*");
                return 6000 + value1;
            }
            return HasFlush(hand);
        }

        private int HasFlush(Card[] hand)
        {
            foreach (Card card in hand)
            {
                int count = 0, sum = card.value;
                foreach (Card card2 in hand)
                {
                    // Excluding the current card itself, count the cards in the hand that have the same suit
                    if (card.value != card2.value && card.suit == card2.suit)
                    {
                        count++;
                        sum += card2.value;
                        if (count == 4)
                        {
                            Console.WriteLine("*Flush*");
                            return 5000 + sum;
                        }
                    }
                }
            }
            return HasStraight(hand);
        }

        private int HasStraight(Card[] hand)
        {
            foreach (Card card in hand)
            {
                int value = card.value;

                // Check for 5 cards of consecutive values
                if (HandContainsValueOnly(hand, value + 1) && HandContainsValueOnly(hand, value + 2) &&
                    HandContainsValueOnly(hand, value + 3) && HandContainsValueOnly(hand, value + 4))
                {
                    Console.WriteLine("*Straight*");
                    return 4000 + (value + 4);
                }
                // Check for A2345
                else if (value == 12)
                {
                    if (HandContainsValueOnly(hand, 0) && HandContainsValueOnly(hand, 1) &&
                        HandContainsValueOnly(hand, 2) && HandContainsValueOnly(hand, 3))
                    {
                        Console.WriteLine("*Straight*");
                        return 4003;
                    }
                }
            }
            return HasThreeOfAKind(hand);
        }

        private int HasThreeOfAKind(Card[] hand)
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
                            Console.WriteLine("*Three Of A Kind*");
                            return 3000 + (3 * card.value) + (SumOfCardValues(hand) - (3 * card.value));
                        }
                    }
                }
            }
            return HasTwoPair(hand);
        }

        private int HasTwoPair(Card[] hand)
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
                Console.WriteLine("*Two Pair*");
                return 2000 + (2 * value1) + (2 * value2) + (SumOfCardValues(hand) - ((2 * value1) + (2 * value2)));
            }
            return HasPair(hand);
        }

        private int HasPair(Card[] hand)
        {
            foreach (Card card in hand)
            {
                foreach (Card card2 in hand)
                {
                    // Check for 2 cards of the same value
                    if (card.suit != card2.suit && card.value == card2.value)
                    {
                        Console.WriteLine("*Pair*");
                        return 1000 + (2 * card.value) + (SumOfCardValues(hand) - (2 * card.value));
                    }
                }
            }
            Console.WriteLine("*High Card*");
            return SumOfCardValues(hand);
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

        // Returns the sum of the values of each card in the given hand
        private int SumOfCardValues(Card[] hand)
        {
            int sum = 0;
            foreach (Card card in hand)
            {
                sum += card.value;
            }
            return sum;
        }
    }
}
