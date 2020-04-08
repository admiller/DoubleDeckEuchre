using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable()]

public class Trick
{

    private List<Card> cards;
    private int highCardSeatNumber;
    private int leadSeatNumber;

    public Trick(int _leadSeatNumber)
    {
        this.cards = new List<Card>();
        this.highCardSeatNumber = -1;
        this.leadSeatNumber = _leadSeatNumber;
    }

    public void PlayCard(Card card)
    {
        // If this trick isn't full yet, add the card.  This assumes that CanPlayCard has been called as validatoin
        if (cards.Count < 4)
        {
            cards.Add(card);
        }
        else
        {
            Debug.LogError("Cannot add card to a trick that has 4 cards already.");
        }

        CalculateHighCardSeatNumber();
    }

    public int CalculateHighCardSeatNumber()
    {
        // Initializing as spades but this will be overwritten when we get to the first card
        int ledSuit = -1;

        // Loop through all cards
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];

            // If this is the first card, track it and set the high card seat to that player's seat
            if (i == 0)
            {
                ledSuit = card.activeSuit;
                highCardSeatNumber = 0;                
            }
            // Cards 2-4
            else
            {
                // Check to see if our card is a higher rank than the highest card to this point
                if (card.rankNumber > cards[highCardSeatNumber].rankNumber)
                {
                    // If this is a trump card, then it MUST be the highest so far
                    if (card.isTrump)
                    {
                        highCardSeatNumber = i;
                    }
                    // If this is not a trump card, then this is only the highest card if we have followed suit
                    else if (card.activeSuit == ledSuit)
                    {
                        highCardSeatNumber = i;
                    }
                }
            }
        }

        return highCardSeatNumber;
    }

    public List<Card> GetCards()
    {
        return cards;
    }

    public int CardCount()
    {
        return cards.Count;
    }

    public int GetWinner()
    {
        if (cards.Count != 4)
        {
            return -1;
        }

        CalculateHighCardSeatNumber();

        // High card seat number is relative to whoever played the first card.  Need to translate to the absolute seat number
        int winnerAbsoluteSeatNumber = (highCardSeatNumber + leadSeatNumber) % 4;

        return winnerAbsoluteSeatNumber;
    }

    public int GetHighCard()
    {
        return highCardSeatNumber;
    }

    public int GetLeadSeatNumber()
    {
        return leadSeatNumber;
    }

    public bool CanPlayCard(Card card, List<Card> hand)
    {
        string debugMessage = "";
        int ledSuit = -1;
        bool ret = true;

        // If a card has been played, log the suit of the first card
        if (cards.Count > 0)
        {
            ledSuit = cards[0].activeSuit;
            //debugMessage = "\nTrying to play the " + Constants.GetCardText(card.cardNumber) + " of " + Constants.GetSuitText(card.suit) + ".  Trump=" + card.isTrump + " activeSuit=" + card.activeSuit
            // + "\n" + RoomHelper.GetPlayerNameBySeatNumber(leadSeatNumber) + " led the " + Constants.GetCardText(cards[0].cardNumber) + " of " + Constants.GetSuitText(cards[0].suit) + ".  Trump=" + cards[0].isTrump;

            // If we are not following suit, then make sure that is ok
            if (card.activeSuit != ledSuit)
            {
                // Loop through all cards in the hand
                foreach (Card c in hand)
                {
                    if (c.hasBeenPlayed == false)
                    {
                        //debugMessage += "\nUnplayed card: The " + Constants.GetCardText(c.cardNumber) + " of " + Constants.GetSuitText(c.suit) + ".  Trump=" + c.isTrump + " activeSuit=" + c.activeSuit;
                    }
                    // If we have a card that is the same suit that was led, then we cannot play off-suit
                    if (c.hasBeenPlayed == false
                    &&  c.activeSuit == ledSuit)
                    {
                        //debugMessage += "\nCannot play card because of the " + Constants.GetCardText(c.cardNumber) + " of " + Constants.GetSuitText(c.suit) + ".  Trump=" + c.isTrump + " activeSuit=" + c.activeSuit;
                        ret = false;
                        break;
                    }
                }
            }

            //debugMessage = "CanPlayCard=" + ret + debugMessage;
            //Debug.Log(debugMessage);
        }

        return ret;
    }
}
