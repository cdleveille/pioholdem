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
                            return 7000 + GetKickerValue(hand, 1);
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
                return 6000 + value1 + value2;
            }
            return HasFlush(hand);
        }

        private int HasFlush(Card[] hand)
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
                            Console.WriteLine("*Flush*");
                            return 5000 + GetKickerValue(hand, 5);
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
                            return 3000 + GetKickerValue(hand, 2);
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
                return 2000 + GetKickerValue(hand, 1);
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
                        return 1000 + GetKickerValue(hand, 3);
                    }
                }
            }
            Console.WriteLine("*High Card*");
            return GetKickerValue(hand, 5);
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

        // Get the sum of the n kicker cards of greatest value
        public int GetKickerValue(Card[] hand, int n)
        {
            int[] valuesToRemove = new int[hand.Length];
            for (int i = 0; i < valuesToRemove.Length; i++)
            {
                valuesToRemove[i] = -1;
            }

            // Find card values that appear more than once within the hand
            for (int i = 0; i < hand.Length; i++)
            {
                foreach (Card card in hand)
                {
                    if (hand[i].suit != card.suit && hand[i].value == card.value)
                    {
                        if (!valuesToRemove.Contains(card.value))
                        {
                            valuesToRemove[i] = hand[i].value;
                        }
                    }
                }
            }

            // Set the value of the duplicate cards to -1 to
            // exclude them from consideration as a kicker
            int removedCount = 0;
            foreach (Card card in hand)
            {
                if (valuesToRemove.Contains(card.value))
                {
                    card.value = -1;
                    removedCount++;
                }
            }

            // Get only the values that are not duplicates
            int[] values = new int[hand.Length - removedCount];
            int trackerIndex = 0;
            foreach (Card card in hand)
            {
                if (card.value != -1)
                {
                    values[trackerIndex] = card.value;
                    trackerIndex++;
                }
            }

            // Sort the values and return the sum of the n greatest values
            int[] sortedValues = values.OrderBy(v => v).ToList().ToArray();
            int kickerValueSum = 0;
            for (int i = sortedValues.Length - 1; i >= sortedValues.Length - n; i--)
            {
                kickerValueSum += sortedValues[i];
            }

            return kickerValueSum;
        }
    }
}
