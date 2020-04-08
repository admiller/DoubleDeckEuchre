using System;
using System.Collections.Generic;

[Serializable()]
public class Deck
{
    public List<Card> cards;

    public Deck()
    {
        cards = new List<Card>();

        // Make a 9 through Ace for all 4 suits
        for (int i = 0; i <= 5; i++)
        {
            // i + 9 makes us use 9, 10, J, Q, K, A
            // i is decknumber 0-5
            Card spade = new Card(i + 9, i, Constants.Spades, Constants.Spades, i + 9, false, false);
            // i + 6 is decknumber 6-11
            Card club = new Card(i + 9, i + 6, Constants.Hearts, Constants.Hearts, i + 9, false, false);
            // i + 12 is decknumber 12-17
            Card heart = new Card(i + 9, i + 12, Constants.Clubs, Constants.Clubs, i + 9, false, false);
            // i +18 is decknumber 18-23
            Card diamond = new Card(i + 9, i + 18, Constants.Diamonds, Constants.Diamonds, i + 9, false, false);

            // Add two copies of each card
            cards.Add(spade);
            cards.Add(spade);
            cards.Add(club);
            cards.Add(club);
            cards.Add(heart);
            cards.Add(heart);
            cards.Add(diamond);
            cards.Add(diamond);
        }
    }

    public void Shuffle()
    {
        System.Random rng = new System.Random();

        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }
    }

    public void SetValuesBasedOnTrump(int trumpSuit)
    {
        foreach (Card c in cards)
        {
            // Reset all cards.  Additionally, if High was the winning bid, this will set rankings accordingly
            c.rankNumber = c.cardNumber;
            c.activeSuit = c.suit;
            c.isTrump = false;

            // If we are playing with a suited trump (not high/low) then handle trump + jacks
            if (trumpSuit != Constants.High
            &&  trumpSuit != Constants.Low)
            {
                // If the card is trump, set it's ranking to 15-20 (9-Ace, Jacks to be dealt with below)
                if (c.suit == trumpSuit)
                {
                    // 6 cards in each suit, this puts the 9 of trump ahead of the Ace of any non-trump suit
                    c.rankNumber = c.cardNumber + 6;
                    c.isTrump = true;
                }

                // If the card is a Jack, check to see if it is a right or left and set values appropriately
                if (c.cardNumber == 11
                // Don't do this if trump is High/Low
                && trumpSuit != Constants.High
                && trumpSuit != Constants.Low)
                {
                    switch (trumpSuit)
                    {
                        case Constants.Spades:
                            // Right (22) will be 2 above the Ace of trump
                            if (c.suit == Constants.Spades)
                            {
                                c.rankNumber = 22;
                                c.isTrump = true;
                            }

                            // Left (21) will be 1 above Ace of trump, and needs its active suit set
                            if (c.suit == Constants.Clubs)
                            {
                                c.rankNumber = 21;
                                c.isTrump = true;
                                c.activeSuit = Constants.Spades;
                            }
                            break;

                        case Constants.Clubs:
                            // Right (22) will be 2 above the Ace of trump
                            if (c.suit == Constants.Clubs)
                            {
                                c.rankNumber = 22;
                                c.isTrump = true;
                            }

                            // Left (21) will be 1 above Ace of trump, and needs its active suit set
                            if (c.suit == Constants.Spades)
                            {
                                c.rankNumber = 21;
                                c.isTrump = true;
                                c.activeSuit = Constants.Clubs;
                            }
                            break;

                        case Constants.Hearts:
                            // Right (22) will be 2 above the Ace of trump
                            if (c.suit == Constants.Hearts)
                            {
                                c.rankNumber = 22;
                                c.isTrump = true;
                            }

                            // Left (21) will be 1 above Ace of trump, and needs its active suit set
                            if (c.suit == Constants.Diamonds)
                            {
                                c.rankNumber = 21;
                                c.isTrump = true;
                                c.activeSuit = Constants.Hearts;
                            }
                            break;

                        case Constants.Diamonds:
                            // Right (22) will be 2 above the Ace of trump
                            if (c.suit == Constants.Diamonds)
                            {
                                c.rankNumber = 22;
                                c.isTrump = true;
                            }

                            // Left (21) will be 1 above Ace of trump, and needs its active suit set
                            if (c.suit == Constants.Hearts)
                            {
                                c.rankNumber = 21;
                                c.isTrump = true;
                                c.activeSuit = Constants.Diamonds;
                            }
                            break;
                    }
                }
            }
            // If we are playing low, flip the rankings so that 9's are best
            else if (trumpSuit == Constants.Low)
            {
                switch (c.cardNumber)
                {
                    case 9:
                        c.rankNumber = 14;
                        break;

                    case 10:
                        c.rankNumber = 13;
                        break;

                    case 11:
                        c.rankNumber = 12;
                        break;

                    case 12:
                        c.rankNumber = 11;
                        break;

                    case 13:
                        c.rankNumber = 10;
                        break;

                    case 14:
                        c.rankNumber = 9;
                        break;
                }
            }
        }
    }
}

