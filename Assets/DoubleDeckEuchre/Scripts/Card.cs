using System;

[Serializable()]
public class Card
{
	// Represents the acutal number of the card (9-14 corresponds to 9-A)
	public int cardNumber;
	// Represents the cards index in a sorted deck (used for loading images easily)
	public int deckNumber;
	// Represents the suit of the card
	public int suit;
	// Represents the active suit of the card (which changes based on what trump currently is)
	public int activeSuit;
	// Represents the cards rank (which changes based on what trump currently is)
	public int rankNumber;
	// Boolean to indicate if this card is currently a member of the Trump suit.  Simplifies the hand organization and trick ranking logic
	public bool isTrump;
	// Boolean to represent if the card has been played yet
	public bool hasBeenPlayed;
	
	public Card(int _cardNumber, int _deckNumber, int _suit, int _activeSuit, int _rankNumber, bool _isTrump, bool _hasBeenPlayed)
	{
		this.cardNumber = _cardNumber;
		this.deckNumber = _deckNumber;
		this.suit = _suit;
		this.activeSuit = _activeSuit;
		this.rankNumber = _rankNumber;
		this.isTrump = _isTrump;
		this.hasBeenPlayed = _hasBeenPlayed;
	}	

}
