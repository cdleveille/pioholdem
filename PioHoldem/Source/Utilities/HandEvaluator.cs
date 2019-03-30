using System;
using System.Linq;

namespace PioHoldem
{
    class HandEvaluator
    {
        // Evaluate hole cards of the given players in combination with the 
        // board and return the index of the player with the best hand, or 
        // return -1 if it is a tie (chop pot)
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

            for (int i = 0; i < players.Length; i++)
            {
                Player player = players[i];

                // Populate the first five cards with the board
                for (int j = 0; j < boardLength; j++)
                {
                    hand[j] = board[j];
                }

                // Populate the last two cards with the current player's hole cards
                hand[boardLength] = player.holeCards[0];
                hand[boardLength + 1] = player.holeCards[1];

                Console.Write(player.name + " shows |" + player.holeCards[0] + "|" + player.holeCards[1] + "| ");

                // Calculate the relative value of the current player's hand
                handValue = GetHandValue(hand);

                // Write the value to the list of hand values for each player
                handValues[i] = handValue;
            }

            // Get the value of the highest ranking hand
            int highestHandValue = handValues.Max();
            int winnerIndex = 0;

            // Check if there is a tie for the highest value
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

        // Calculate the relative value of the given hand using the helper methods below
        public int GetHandValue(Card[] hand)
        {
            hand = SortByValueDescending(hand);

            // Start by checking for the highest possible 
            // hand rank and working down from there
            return HasRoyalFlush(hand);
        }

        private int HasRoyalFlush(Card[] hand)
        {
            foreach (Card card in hand)
            {
                int suit = card.suit;
                int value = card.value;
                
                // Check for TJQKA of same suit
                if (value == 8)
                {
                    if (HandContains(hand, suit, 9) && HandContains(hand, suit, 10) &&
                        HandContains(hand, suit, 11) && HandContains(hand, suit, 12))
                    {
                        Console.WriteLine("*Royal Flush*");
                        return 800000012;
                    }
                }
            }
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
                    return 800000000 + (value + 4);
                }
                // Check for A2345 of same suit
                else if (value == 12)
                {
                    if (HandContains(hand, suit, 0) && HandContains(hand, suit, 1) &&
                        HandContains(hand, suit, 2) && HandContains(hand, suit, 3))
                    {
                        Console.WriteLine("*Straight Flush*");
                        return 800000003;
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
                            // Give the quads card more weight than the kicker card
                            return 700000000 + (500000 * card.value) + GetKickerValue(hand, 1);
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
                            break;
                        }
                    }
                    // If 3 cards of the same value are found, do not continue searching
                    if (value1 >= 0)
                    {
                        break;
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
                        break;
                    }
                }
                // If a pair is found, do not continue searching
                if (value2 >= 0)
                {
                    break;
                }
            }
            if (value1 >= 0 && value2 >= 0)
            {
                Console.WriteLine("*Full House*");
                return 600000000 + (500000 * value1) + value2;
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
                            return 500000000 + GetKickerValue(hand, 5);
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
                    return 400000000 + (value + 4);
                }
                // Check for A2345
                else if (value == 12)
                {
                    if (HandContainsValueOnly(hand, 0) && HandContainsValueOnly(hand, 1) &&
                        HandContainsValueOnly(hand, 2) && HandContainsValueOnly(hand, 3))
                    {
                        Console.WriteLine("*Straight*");
                        return 400000003;
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
                            return 300000000 + (500000 * card.value) + GetKickerValue(hand, 2);
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
                        break;
                    }
                }
                // If a pair is found, do not continue searching
                if (value1 >= 0)
                {
                    break;
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
                        break;
                    }
                }
                // If a pair is found, do not continue searching
                if (value2 >= 0)
                {
                    break;
                }
            }
            if (value1 >= 0 && value2 >= 0)
            {
                Console.WriteLine("*Two Pair*");
                return 200000000 + (7000000 * value1) + (500000 * value2) + GetKickerValue(hand, 1);
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
                        return 100000000 + (500000 * card.value) + GetKickerValue(hand, 3);
                    }
                }
            }
            Console.WriteLine("*High Card*");
            return GetKickerValue(hand, 5);
        }

        // Return true if the given hand contains a card of the given suit and value
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

        // Return true if the given hand contains a card of the given value
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

        // Get the weighted sum of the n kicker cards of greatest value
        private int GetKickerValue(Card[] hand, int n)
        {
            // Find card values that appear more than once within the hand
            // (not included in the kicker value). Only remove 2 maximum.
            int[] pairedValues = new int[hand.Length];
            int removedCount = 0;
            for (int i = 0; i < hand.Length; i++)
            {
                foreach (Card card in hand)
                {
                    if (hand[i].suit != card.suit && hand[i].value == card.value)
                    {
                        if (!pairedValues.Contains(card.value) && removedCount < 2)
                        {
                            pairedValues[i] = hand[i].value;
                            removedCount++;
                        }
                    }
                }
            }

            // Get only the values that are not duplicates
            int[] values = new int[hand.Length - removedCount];
            int trackerIndex = 0;
            foreach (Card card in hand)
            {
                if (!pairedValues.Contains(card.value))
                {
                    // Add 1 to the value so that 2s are not value of 0
                    values[trackerIndex] = card.value + 1;
                    trackerIndex++;
                }
            }

            // Sort the values and return the weighted sum of the n greatest values
            int[] sortedValues = values.OrderBy(v => v).ToList().ToArray();
            int kickerValueSum = 0, weight;
            for (int i = sortedValues.Length - 1; i >= sortedValues.Length - n; i--)
            {
                // Give the kicker card of greatest value the most weight
                if (i == sortedValues.Length - 1)
                {
                    weight = 36000;
                }
                else if (i == sortedValues.Length - 2)
                {
                    weight = 2700;
                }
                else if (i == sortedValues.Length - 3)
                {
                    weight = 200;
                }
                else if ((i == sortedValues.Length - 4))
                {
                    weight = 15;
                }
                else
                {
                    weight = 1;
                }
                kickerValueSum += weight * sortedValues[i];
            }

            return kickerValueSum;
        }

        // Sort the given list of cards in descending order by value
        private Card[] SortByValueDescending(Card[] cards)
        {
            Card[] sortedCards = new Card[cards.Length];
            for (int i = 0; i < cards.Length; i++)
            {
                int maxValueCardIndex = GetMaxValueCardIndex(cards);
                sortedCards[i] = new Card(cards[maxValueCardIndex].suit, cards[maxValueCardIndex].value);
                cards[maxValueCardIndex] = null;
            }
            return sortedCards;
        }

        // Return the index of the card with the highest value in the given list
        private int GetMaxValueCardIndex(Card[] cards)
        {
            int maxValueCardIndex = 0;
            int maxValue = 0;
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i] != null)
                {
                    if (cards[i].value >= maxValue)
                    {
                        maxValue = cards[i].value;
                        maxValueCardIndex = i;
                    }
                }
            }
            return maxValueCardIndex;
        }

        // Return a simplified string version of the given hole cards
        // Examples: JcTc => JTs
        //           8s9h => 98o
        //           KhKd => KK
        public string ClassifyHoleCards(Card[] holeCards)
        {
            Card[] sortedHoleCards = new Card[] {new Card(holeCards[0].suit, holeCards[0].value), new Card(holeCards[1].suit, holeCards[1].value) };
            sortedHoleCards = SortByValueDescending(sortedHoleCards);
            string holeCardString = sortedHoleCards[0].ToStringValueOnly() + sortedHoleCards[1].ToStringValueOnly();

            if (sortedHoleCards[0].value != sortedHoleCards[1].value)
            {
                if (sortedHoleCards[0].suit != sortedHoleCards[1].suit)
                {
                    holeCardString += "o";
                }
                else
                {
                    holeCardString += "s";
                }
            }
            return holeCardString;
        }
    }
}
