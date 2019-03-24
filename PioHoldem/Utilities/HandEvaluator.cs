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

                hand = SortByValueDescending(hand);

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

        // Calculate the relative value of the given
        // hand using the helper methods below
        public int GetHandValue(Card[] hand)
        {
            // Start by checking for the highest possible 
            // hand rank and working down from there
            return HasStraightFlush(hand);
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
                        return 8012;
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
                            // Give the quads card more weight than the kicker card
                            return 7000 + (50 * card.value) + GetKickerValue(hand, 1);
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
                            // Give the trips card more weight than the kicker cards
                            return 3000 + (50 * card.value) + GetKickerValue(hand, 2);
                        }
                    }
                }
            }
            return HasTwoPair(hand);
        }

        // KNOWN BUG:
        // Showdown!
        // Board: |4h|8s|Qh|4c|8d|
        // Salmon shows |9c|9d| *Two Pair*
        // Trout shows |2c|Ks| *Two Pair*
        // Trout wins pot of 40

        // Showdown!
        // Board: |7s|6c|6s|5s|7c|
        // Human shows |Qh|Kc| *Two Pair*
        // SharkAI shows |5d|Ah| *Two Pair*
        // Human wins pot of 400
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
                // Give the two pair cards more weight than the kicker cards
                return 2000 + (50 * value1) + (15 * value2) + GetKickerValue(hand, 1);
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
                        return 1000 + (50 * card.value) + GetKickerValue(hand, 3);
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

        // Get the sum of the n kicker cards of greatest value
        private int GetKickerValue(Card[] hand, int n)
        {
            int[] valuesToRemove = new int[hand.Length];

            // Find card values that appear more than once within the hand
            // (not included in the kicker value)
            int removedCount = 0;
            for (int i = 0; i < hand.Length; i++)
            {
                foreach (Card card in hand)
                {
                    if (hand[i].suit != card.suit && hand[i].value == card.value)
                    {
                        if (!valuesToRemove.Contains(card.value))
                        {
                            valuesToRemove[i] = hand[i].value;
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
                if (!valuesToRemove.Contains(card.value))
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
