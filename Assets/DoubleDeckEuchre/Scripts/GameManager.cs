using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Realtime;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;
using System;

namespace ADM.DoubleDeckEuchre
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public GameObject card1;
        public GameObject card2;
        public GameObject card3;
        public GameObject card4;
        public GameObject card5;
        public GameObject card6;
        public GameObject card7;
        public GameObject card8;
        public GameObject card9;
        public GameObject card10;
        public GameObject card11;
        public GameObject playerHandArea;

        public GameObject kitty1;
        public GameObject kitty2;
        public GameObject kitty3;
        public GameObject kitty4;
        public GameObject kittyArea;

        public GameObject playedCards1;
        public GameObject playedCards2;
        public GameObject playedCards3;
        public GameObject playedCards4;
        public GameObject playedCardsArea;

        public InputField bidNumberInputField;
        public Dropdown bidSuitDropdown;

        public GameObject bidArea;

        public GameObject dealButton;

        public Text playerLeftNameText;
        public Text playerTopNameText;
        public Text playerRightNameText;
        public Text playerNameText;
        public Text playerLeftBidText;
        public Text playerTopBidText;
        public Text playerRightBidText;
        public Text playerBidText;
        public Text teamOneScoreText;
        public Text teamTwoScoreText;
        public Text teamOneTricksText;
        public Text teamTwoTricksText;
        public Text gameMessageText;
        public Text currentTrumpText;

        public Material greyMaterial;

        private List<GameObject> cardObjects;
        private List<GameObject> playedCardObjects;
        private List<GameObject> kittyObjects;

        private List<Player> playerList;

        private Deck deck;
        private List<Card> hand;

        private int seatNumber;

        private Texture[] cardTextures;

        private Trick trick;

        private Bid winningBid;
        private BidList bidList;
        private bool kittyPhase;
        private List<int> discardedCards;

        // Start Method
        void Start()
        {
            if (!PhotonNetwork.IsConnected) // 1
            {
                SceneManager.LoadScene("Launcher");
                return;
            }

            // Note: Not doing error checking here because if this is empty, everything is broken
            seatNumber = (int)PhotonNetwork.LocalPlayer.CustomProperties["SeatNumber"];

            // If deck object is null, create it
            if (deck == null)
            {
                deck = new Deck();
            }

            if (cardTextures == null)
            {
                cardTextures = new[] {
                    Resources.Load<Texture>("PlayingCards/9_of_spades"),
                    Resources.Load<Texture>("PlayingCards/10_of_spades"),
                    Resources.Load<Texture>("PlayingCards/jack_of_spades"),
                    Resources.Load<Texture>("PlayingCards/queen_of_spades"),
                    Resources.Load<Texture>("PlayingCards/king_of_spades"),
                    Resources.Load<Texture>("PlayingCards/ace_of_spades"),

                    Resources.Load<Texture>("PlayingCards/9_of_hearts"),
                    Resources.Load<Texture>("PlayingCards/10_of_hearts"),
                    Resources.Load<Texture>("PlayingCards/jack_of_hearts"),
                    Resources.Load<Texture>("PlayingCards/queen_of_hearts"),
                    Resources.Load<Texture>("PlayingCards/king_of_hearts"),
                    Resources.Load<Texture>("PlayingCards/ace_of_hearts"),

                    Resources.Load<Texture>("PlayingCards/9_of_clubs"),
                    Resources.Load<Texture>("PlayingCards/10_of_clubs"),
                    Resources.Load<Texture>("PlayingCards/jack_of_clubs"),
                    Resources.Load<Texture>("PlayingCards/queen_of_clubs"),
                    Resources.Load<Texture>("PlayingCards/king_of_clubs"),
                    Resources.Load<Texture>("PlayingCards/ace_of_clubs"),


                    Resources.Load<Texture>("PlayingCards/9_of_diamonds"),
                    Resources.Load<Texture>("PlayingCards/10_of_diamonds"),
                    Resources.Load<Texture>("PlayingCards/jack_of_diamonds"),
                    Resources.Load<Texture>("PlayingCards/queen_of_diamonds"),
                    Resources.Load<Texture>("PlayingCards/king_of_diamonds"),
                    Resources.Load<Texture>("PlayingCards/ace_of_diamonds"),
                    };
            }

            // Load all local player cardobjects into the list for rendering
            cardObjects = new List<GameObject>();
            cardObjects.Add(card1);
            cardObjects.Add(card2);
            cardObjects.Add(card3);
            cardObjects.Add(card4);
            cardObjects.Add(card5);
            cardObjects.Add(card6);
            cardObjects.Add(card7);
            cardObjects.Add(card8);
            cardObjects.Add(card9);
            cardObjects.Add(card10);
            cardObjects.Add(card11);
            hand = new List<Card>();

            // Add kitty cards
            kittyObjects = new List<GameObject>();
            kittyObjects.Add(kitty1);
            kittyObjects.Add(kitty2);
            kittyObjects.Add(kitty3);
            kittyObjects.Add(kitty4);

            // Add Played cards
            playedCardObjects = new List<GameObject>();
            playedCardObjects.Add(playedCards1);
            playedCardObjects.Add(playedCards2);
            playedCardObjects.Add(playedCards3);
            playedCardObjects.Add(playedCards4);

            // Instantiate bidlist
            bidList = new BidList();

            // Disable PlayerHand and Kitty areas until cards are dealt
            playerHandArea.SetActive(false);
            kittyArea.SetActive(false);
            playedCardsArea.SetActive(false);
            bidArea.SetActive(false);

            // Disable this button globally.  It will be reenabled when the custom properties callback is proc'd
            dealButton.SetActive(false);

            // Build list of players relative to ourselves
            playerList = new List<Player>();
            ArrayList allPlayers = RoomHelper.GetTeamOnePlayers();
            allPlayers.AddRange(RoomHelper.GetTeamTwoPlayers());
            Player playerLeft = null, playerTop = null, playerRight = null;
            foreach (int actorId in allPlayers)
            {
                Player p = PhotonNetwork.CurrentRoom.GetPlayer(actorId);
                int playerSeatNumber = (int) p.CustomProperties["SeatNumber"];

                // Calculate relative seat number based on the local player's seat
                int relativeSeatNumber = (playerSeatNumber - seatNumber + 4) % 4;

                switch (relativeSeatNumber)
                {
                    case 0:
                        playerList.Add(PhotonNetwork.LocalPlayer);
                        playerBidText.text = "Bid: ";
                        break;

                    case 1:
                        playerLeft = p;
                        playerLeftNameText.text = p.NickName;
                        playerLeftBidText.text = "Bid: ";
                        break;

                    case 2:
                        playerTop = p;
                        playerTopNameText.text = p.NickName;
                        playerTopBidText.text = "Bid: ";
                        break;

                    case 3:
                        playerRight = p;
                        playerRightNameText.text = p.NickName;
                        playerRightBidText.text = "Bid: ";
                        break;

                    default:
                        break;
                }
            }

            // Set our name at the bottom of the screen
            playerNameText.text = RoomHelper.GetPlayerNameBySeatNumber(seatNumber);

            // We already added ourselves, now we add the others in a rotating circle
            playerList.Add(playerLeft);
            playerList.Add(playerTop);
            playerList.Add(playerRight);            

            // TODO maybe randomly assign the deal?
            Hashtable ht = new Hashtable();
            ht.Add("DealerSeatNumber", 0);
            ht.Add("BidSeatNumber", -1);
            ht.Add("TeamOneScore", 0);
            ht.Add("TeamTwoScore", 0);
            ht.Add("GameMessage", RoomHelper.GetPlayerNameBySeatNumber(0) + "'s turn to deal!");
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
        }

        // Deal Cards
        public void Deal()
        {
            // Shuffle deck
            deck = new Deck();
            deck.Shuffle();

            // Create the first Trick object
            trick = new Trick(seatNumber);

            // Set properties to send to the server
            Hashtable ht = new Hashtable();

            // Serialize the Deck object
            ht.Add("Deck", Serializer.SerializeDeck(deck));

            // Give the bid to the player to our left
            ht.Add("BidSeatNumber", (seatNumber + 1) % 4);

            bidList = new BidList();
            ht.Add("BidList", Serializer.SerializeBidList(bidList));

            // Serialize the empty Trick
            ht.Add("Trick", Serializer.SerializeTrick(trick));

            // Reset tricks for both teams
            ht.Add("TeamOneTricks", 0);
            ht.Add("TeamTwoTricks", 0);

            // Reset the bid
            ht.Add("WinningBid", Serializer.SerializeBid(Bid.Construct(-1, "", -1)));

            ht.Add("GameMessage", RoomHelper.GetPlayerNameBySeatNumber((seatNumber + 1) % 4) + " has the first bid");

            // Send properties to server
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
        }

        public void OnBidClicked()
        {            
            Hashtable customProperties = RoomHelper.GetRoomCustomProperties();

            // Get the values entered by the user
            int bidTrickNumber = Int32.Parse(bidNumberInputField.text);
            string bidSuit = bidSuitDropdown.options[bidSuitDropdown.value].text;

            // Make sure they can't do something really stupid
            if (bidTrickNumber < 0
            ||  bidTrickNumber > 11)
            {
                gameMessageText.text = "Illegal bid number";
                return;
            }

            // Check to see if there is already a bid
            if (customProperties.ContainsKey("WinningBid"))
            {
                winningBid = Serializer.DeserializeBid((byte[])customProperties["WinningBid"]);

                if (winningBid != null)
                {
                    // Make sure that we are bidding higher than the winning bid
                    if (bidTrickNumber <= winningBid.trickNumber)
                    {
                        gameMessageText.text = "Must bid higher than " + winningBid.trickNumber;
                        return;
                    }
                }
            }

            // Construct bid object
            Bid bid = Bid.Construct(bidTrickNumber, bidSuit, seatNumber);

            // Add bid to the local list
            bidList.AddBid(bid);

            // Set our bid to be the winning bid and serialize the bidlist to send to the server
            Hashtable ht = new Hashtable();
            winningBid = bid;
            ht.Add("WinningBid", Serializer.SerializeBid(winningBid));
            ht.Add("BidList", Serializer.SerializeBidList(bidList));

            // If we have bid 4 times, set the next bid to -1 and give the bid winner the first play
            if (bidList.GetBids().Count >= 4)
            {
                ht.Add("BidSeatNumber", -1);
                ht.Add("CurrentTurnSeatNumber", winningBid.seatNumber);

                // Add a message so people see who won
                ht.Add("GameMessage", RoomHelper.GetPlayerNameBySeatNumber(seatNumber) + " wins the bid with " + bid.trickNumber + " " + bid.suitName);

                // Make a new Trick object with the correct Seat Number
                trick = new Trick(seatNumber);
                ht.Add("Trick", Serializer.SerializeTrick(trick));
            }
            // Set the next player to be the bidder
            else
            {
                ht.Add("BidSeatNumber", (seatNumber + 1) % 4);

                // Add a message so people notice that a bid happened
                ht.Add("GameMessage", RoomHelper.GetPlayerNameBySeatNumber(seatNumber) + " bids " + bid.trickNumber + " " + bid.suitName);
            }            

            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
        }

        public void OnPassClicked()
        {
            Hashtable customProperties = RoomHelper.GetRoomCustomProperties();

            // Construct bid object
            Bid bid = Bid.Construct(0, "Pass", seatNumber);

            // Add bid to the local list
            bidList.AddBid(bid);

            // Serialize the bidlist to send to the server
            Hashtable ht = new Hashtable();
            ht.Add("BidList", Serializer.SerializeBidList(bidList));

            // If we have bid 4 times, set the next bid to -1 and give the bid winner the first play
            // TODO add kitty
            if (bidList.GetBids().Count >= 4)
            {
                ht.Add("BidSeatNumber", -1);
                ht.Add("CurrentTurnSeatNumber", winningBid.seatNumber);

                // Add a message so people see who won
                ht.Add("GameMessage", RoomHelper.GetPlayerNameBySeatNumber(winningBid.seatNumber) + " wins the bid with " + winningBid.trickNumber + " " + winningBid.suitName);

                // Make a new Trick object with the correct Seat Number
                trick = new Trick(winningBid.seatNumber);
                ht.Add("Trick", Serializer.SerializeTrick(trick));
            }
            // Set the next player to be the bidder
            else
            {
                ht.Add("BidSeatNumber", (seatNumber + 1) % 4);

                // Add a message so people notice that a pass happened
                ht.Add("GameMessage", RoomHelper.GetPlayerNameBySeatNumber(seatNumber) + " passes");
            }            

            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);            
        }

        public void ReceiveCards()
        {
            // Instantiate Hand list (11 cards)
            hand = new List<Card>();

            // First player gets first 11 cards, second player gets 12-22, etc.  Using this offset to traverse the Deck array
            int seatNumberOffset = seatNumber * 11;

            // Assign the 11 Cards from the Deck object to this player's Hand
            for (int i = 0; i < 11; i++)
            {
                Card card = deck.cards[i + seatNumberOffset];
                hand.Add(card);
            }

            // Organize and display the hand
            OrganizeHand();

            // Show player cards
            playerHandArea.SetActive(true);

            // TODO figure out how to hide individual cards by tracking whose turn it is

            // Hide the Deal button
            dealButton.SetActive(false);
        }

        public void OrganizeHand()
        {
            // Sort by trump, then suit, then card rank
            hand = hand.OrderBy(c => c.isTrump).ThenByDescending(c => c.activeSuit).ThenByDescending(c => c.rankNumber).ToList();

            // Loop through the 11 cards and add the images based on the deck
            for (int i = 0; i < hand.Count; i++)
            {
                // Get the UI card object
                GameObject cardGameObject = cardObjects[i];
                cardGameObject.SetActive(true);

                // The relevant card texture is stored in an array of textures.  The Card object (inside the deck/hand) stores this index for us
                cardGameObject.GetComponent<RawImage>().texture = cardTextures[hand[i].deckNumber];
            }
        }

        /// <summary>
        /// Greys out any cards that can't be played at the moment given the current trick
        /// </summary>
        public void DisableUnplayableCards()
        {
            for (int i = 0; i < hand.Count; i++)
            {
                // Get the card game object
                GameObject cardGameObject = cardObjects[i];

                // By default, set the background to white
                cardGameObject.GetComponent<RawImage>().material = null;

                // If there is already a trick, then we may have cards that may need disabled
                if (trick != null)
                {
                    // If this card hasn't been played yet, then we care about the color
                    if (cardGameObject.activeSelf)
                    {
                        // Get the card in our hand at this location
                        Card c = hand[i];

                        // If we can't play this card, then grey it out
                        if (!trick.CanPlayCard(c, hand))
                        {
                            cardGameObject.GetComponent<RawImage>().material = greyMaterial;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Sets the text values for the player bids
        /// </summary>
        public void SetPlayerBidTexts()
        {
            // Clear out all of the text's before we do any looping
            playerBidText.text = "Bid: ";
            playerLeftBidText.text = "Bid: ";
            playerRightBidText.text = "Bid: ";
            playerTopBidText.text = "Bid: ";

            // Loop through all bids up to this position
            foreach (Bid b in bidList.GetBids())
            {
                // Get absolute seat number for this bid
                int bidSeatNumber = b.seatNumber;

                // Calculate relative seat number based on the local player's seat
                int relativeSeatNumber = (bidSeatNumber - seatNumber + 4) % 4;

                switch (relativeSeatNumber)
                {
                    case 0:
                        playerBidText.text = "Bid: " + b.trickNumber + " " + b.suitName;
                        break;

                    case 1:
                        playerLeftBidText.text = "Bid: " + b.trickNumber + " " + b.suitName;
                        break;

                    case 2:
                        playerTopBidText.text = "Bid: " + b.trickNumber + " " + b.suitName;
                        break;

                    case 3:
                        playerRightBidText.text = "Bid: " + b.trickNumber + " " + b.suitName;
                        break;

                    default:
                        break;
                }
            }            
        }


        // Update Method
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) //1
            {
                Application.Quit();
            }
        }

        /// <summary>
        /// Event for a player leaving the room
        /// </summary>
        /// <param name="other">The player who closed the application (or dc'd)</param>
        public override void OnPlayerLeftRoom(Player other)
        {
            //Debug.Log("OnPlayerLeftRoom() " + other.NickName); // seen when other disconnects
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("Launcher");
            }
        }
                
        public void QuitRoom()
        {
            Application.Quit();
        }

        /// <summary>
        /// Helper method to return a team number for a player in a given seat (absolute)
        /// </summary>
        /// <param name="playerSeatNumber"></param>
        /// <returns></returns>
        public int GetTeamNumberBySeatNumber(int playerSeatNumber)
        {
            if (playerSeatNumber == 0
            ||  playerSeatNumber == 2)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        /// <summary>
        /// Checks the custom properties list to see if we are currently bidding
        /// </summary>
        /// <returns>True if bidding has yet to complete</returns>
        public bool IsInBidPhase()
        {
            Hashtable customProperties = RoomHelper.GetRoomCustomProperties();

            if (customProperties.ContainsKey("BidSeatNumber"))
            {
                return (int)customProperties["BidSeatNumber"] >= 0;
            }

            return false;
        } 

        /// <summary>
        /// Handles cards being double clicked (the main form of interaction with the cards).
        /// This handles playing cards, discarding cards (after the kitty) and sends the data back to the server to be processed by all clients
        /// </summary>
        /// <param name="index">The index of the card that was double clicked (0-14 which includes the player's hand and possible Kitty cards)</param>
        /// <param name="cardObject">The GameObject that triggered the event</param>
        public void OnCardDoubleClicked(int index, GameObject cardObject)
        {
            // Handle clicking of card during kitty phase that was won by the local player
            if (kittyPhase && winningBid.seatNumber == seatNumber)
            {
                // Add the card index to the list of discarded cards
                discardedCards.Add(index);

                // If the card came from our hand, remove it
                if (index < 11)
                {
                    cardObjects[index].SetActive(false);
                }
                // If the card was in the kitty, remove it
                else
                {
                    kittyObjects[index - 11].SetActive(false);
                }

                // If we have discarded 4 cards, then remove those cards from the Hand list.
                if (discardedCards.Count >= 4)
                {
                    // Go descending to not hit index issues
                    foreach (int i in discardedCards.OrderByDescending(x => x))
                    {
                        hand.RemoveAt(i);
                    }

                    // Disable all the kitty card objects
                    for (int i = 0; i < 4; i++)
                    {
                        kittyObjects[i].SetActive(false);
                    }

                    OrganizeHand();

                    // Turn off kitty phase
                    kittyPhase = false;

                    // Show the card playing area for the player who was discarding
                    playedCardsArea.SetActive(true);

                    // Send an update that the kitty phase is complete and let the players know who goes first
                    Hashtable ht = new Hashtable();
                    ht.Add("GameMessage", "Discarding complete. " + RoomHelper.GetPlayerNameBySeatNumber(seatNumber) + " has the lead.");
                    
                    // Send properties to the server
                    PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
                }
            }
            // Handle clicking a card in our own hand during the actual play phase (not during bidding)
            else if (index < hand.Count
            && !kittyPhase
            && !IsInBidPhase())
            {
                Card card = hand[index];

                Hashtable customProperties = RoomHelper.GetRoomCustomProperties();

                // If it is our turn, play the card
                if ((int)customProperties["CurrentTurnSeatNumber"] == seatNumber)
                {
                    // Make sure this is a legal play
                    if (trick.CanPlayCard(card, hand))
                    {
                        // Remove the card from our hand
                        cardObjects[index].SetActive(false);

                        // Send our played card and our seat number to the server for processing
                        Hashtable ht = new Hashtable();
                        ht.Add("CardPlayedSeatNumber", seatNumber);
                        ht.Add("CardPlayed", Serializer.SerializeCard(card));

                        // Flag the card as played
                        card.hasBeenPlayed = true;

                        // Add our card to the trick
                        trick.PlayCard(card);

                        // Check to see if this was the last card in the trick
                        if (trick.CardCount() == 4)
                        {
                            // Get the winner's seat number (absolute)
                            int winnerSeatNumber = trick.GetWinner();

                            // Get the winning team number
                            int winnerTeamNumber = GetTeamNumberBySeatNumber(winnerSeatNumber);

                            // Get the number of won tricks up to this point
                            int teamOneTricks = (int)customProperties["TeamOneTricks"];
                            int teamTwoTricks = (int)customProperties["TeamTwoTricks"];

                            // Team one won. Increment trick counter
                            if (winnerTeamNumber == 1)
                            {
                                teamOneTricks++;
                                ht.Add("TeamOneTricks", teamOneTricks);
                            }
                            // Team two won. Increment trick counter
                            else
                            {
                                teamTwoTricks++;
                                ht.Add("TeamTwoTricks", teamTwoTricks);
                            }

                            // Send a game message about who won that trick                            
                            Card winningCard = trick.GetCards()[trick.GetHighCard()];
                            ht.Add("GameMessage", RoomHelper.GetPlayerNameBySeatNumber(winnerSeatNumber) + " won the trick with the " + Constants.GetCardText(winningCard.cardNumber) + " of " + Constants.GetSuitText(winningCard.suit));

                            // Give the lead to whoever won the trick
                            ht.Add("CurrentTurnSeatNumber", winnerSeatNumber);

                            // Create a new trick and reset the lead
                            trick = new Trick(winnerSeatNumber);

                            // If the round is over, add the points and move to the next deal
                            if (teamOneTricks + teamTwoTricks >= 11)
                            {
                                // Get current scores
                                int teamOneScore = (int)customProperties["TeamOneScore"];
                                int teamTwoScore = (int)customProperties["TeamTwoScore"];

                                // If team 1 won, check to see if they made their bid
                                if (winningBid.seatNumber == 0
                                ||  winningBid.seatNumber == 2)
                                {
                                    if (teamOneTricks >= winningBid.trickNumber)
                                    {
                                        teamOneScore += teamOneTricks;
                                    }
                                    // Team one was euchred, they go negative
                                    else
                                    {
                                        teamOneScore -= winningBid.trickNumber;
                                    }

                                    // Team 1 had the bid, so Team 2's score simply increments by their won tricks
                                    teamTwoScore += teamTwoTricks;
                                }
                                // If team 2 won, check to see if they made their bid
                                else
                                {
                                    if (teamTwoTricks >= winningBid.trickNumber)
                                    {
                                        teamTwoScore += teamTwoTricks;
                                    }
                                    // Team two was euchred, they go negative
                                    else
                                    {
                                        teamTwoScore -= winningBid.trickNumber;
                                    }

                                    // Team 2 had the bid, so Team 1's score simply increments by their won tricks
                                    teamOneScore += teamOneTricks;
                                }

                                // Send out the new scores to the server
                                ht.Add("TeamOneScore", teamOneScore);
                                ht.Add("TeamTwoScore", teamTwoScore);

                                // Move the deal
                                int dealerSeatNumber = (int)customProperties["DealerSeatNumber"];
                                dealerSeatNumber = (dealerSeatNumber + 1) % 4;
                                ht.Add("DealerSeatNumber", dealerSeatNumber);

                                bidList = new BidList();
                                ht.Add("BidList", Serializer.SerializeBidList(bidList));
                            }
                        }
                        // Trick isn't finished yet
                        else
                        {
                            // Increment currentTurnSeatNumber
                            int nextTurnSeatNumber = ((int)customProperties["CurrentTurnSeatNumber"] + 1) % 4;
                            ht.Add("CurrentTurnSeatNumber", nextTurnSeatNumber);

                            // Blank out the game message if we are just playing cards
                            ht.Add("GameMessage", "");
                        }

                        // Put the trick in the hashtable.  This will either be the trick with our card added, or a new one if we just played the 4th card
                        ht.Add("Trick", Serializer.SerializeTrick(trick));

                        // Send properties to the server
                        PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
                    }
                    // Illegal move
                    else
                    {
                        gameMessageText.text = "Cannot play the " + Constants.GetCardText(card.cardNumber) + " of " + Constants.GetSuitText(card.suit);
                    }
                }
                // It is not our turn, show message in the UI
                else
                {
                    gameMessageText.text = "Not your turn.  Card cannot be played";
                }
            }
        }

        /// <summary>
        /// Hook for Photon Room Properties being updated.  This is the main way that data is passed between clients
        /// </summary>
        /// <param name="propertiesThatChanged"></param>
        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            // See if cards were just dealt
            if (propertiesThatChanged.ContainsKey("Deck"))
            {
                deck = (Deck)Serializer.DeserializeDeck((byte[])propertiesThatChanged["Deck"]);
                ReceiveCards();
                DisableUnplayableCards();
                currentTrumpText.text = "";

                for (int i = 0; i < playedCardObjects.Count; i++)
                {
                    playedCardObjects[i].GetComponent<RawImage>().texture = null;                   
                }
            }

            // See if it is time for a new dealer
            if (propertiesThatChanged.ContainsKey("DealerSeatNumber"))
            {
                // If we are the current dealer, show the deal button
                if ((int)propertiesThatChanged["DealerSeatNumber"] == seatNumber)
                {
                    dealButton.SetActive(true);
                }
                // Hide the deal button
                else
                {
                    dealButton.SetActive(false);
                }
            }

            // Handle a new high bid
            if (propertiesThatChanged.ContainsKey("WinningBid"))
            {
                winningBid = (Bid)Serializer.DeserializeBid((byte[])propertiesThatChanged["WinningBid"]);
            }

            // Handle bid set number updates
            if (propertiesThatChanged.ContainsKey("BidSeatNumber"))
            {
                // Our turn to bid
                if ((int)propertiesThatChanged["BidSeatNumber"] == seatNumber)
                {
                    bidArea.SetActive(true);
                }
                // Not our turn to bid
                else
                {
                    bidArea.SetActive(false);
                }

                // If we are still bidding (since the bid seat number is a valid number) then keep the playing area empty
                if ((int)propertiesThatChanged["BidSeatNumber"] >= 0)
                {
                    playedCardsArea.SetActive(false);
                }
                // Bidding has ended.  Show the playing cards area (assuming we've even started the game)
                else if (hand.Count > 0)
                {
                    playedCardsArea.SetActive(true);

                    // Reorganize the card rankings inside the deck
                    deck.SetValuesBasedOnTrump(winningBid.suitNumber);

                    // Reorganize hand
                    OrganizeHand();

                    // Set our trump text at the top
                    currentTrumpText.text = "Trump: " + winningBid.trickNumber + " " + winningBid.suitName + " by " + RoomHelper.GetPlayerNameBySeatNumber(winningBid.seatNumber);

                    // We won the bid
                    if (winningBid.seatNumber == seatNumber)
                    {
                        // Set kittyphase flag
                        kittyPhase = true;

                        // Add the 4 kitty cards to our hand list
                        hand.Add(deck.cards[44]);
                        hand.Add(deck.cards[45]);
                        hand.Add(deck.cards[46]);
                        hand.Add(deck.cards[47]);

                        // Set the kitty area to active
                        kittyObjects[0].SetActive(true);
                        kittyObjects[1].SetActive(true);
                        kittyObjects[2].SetActive(true);
                        kittyObjects[3].SetActive(true);

                        kittyArea.SetActive(true);

                        // Hide the playing area for this local player
                        playedCardsArea.SetActive(false);

                        // Loop through the 4 objects and add the appropriate textures to them
                        for (int i = 0; i < 4; i++)
                        {
                            kittyObjects[i].GetComponent<RawImage>().texture = cardTextures[hand[i + 11].deckNumber];
                        }

                        // Instantiate the discarded cards list
                        discardedCards = new List<int>();

                        gameMessageText.text = "Discard 4 cards by double clicking.";
                    }
                }
            }

            // Handle a new bid/pass and save it to the local list
            if (propertiesThatChanged.ContainsKey("BidList"))
            {
                bidList = (BidList)Serializer.DeserializeBidList((byte[])propertiesThatChanged["BidList"]);

                // Update the bid text boxes
                SetPlayerBidTexts();
            }

            // Trick was updated.  This happens when a card is played, or a previous trick completes
            if (propertiesThatChanged.ContainsKey("Trick"))
            {
                // Deserialize this and keep it locally.  This keeps everyone's trick object in sync so that trick calculations can be done locally
                trick = Serializer.DeserializeTrick((byte[])propertiesThatChanged["Trick"]);

                // Redraw unplayable cards
                DisableUnplayableCards();
            }

            // A card was played (either by another player, or by the local player)
            if (propertiesThatChanged.ContainsKey("CardPlayed")
            && propertiesThatChanged.ContainsKey("CardPlayedSeatNumber"))
            {
                // Get the Card object that was just played
                Card card = Serializer.DeserializeCard((byte[])propertiesThatChanged["CardPlayed"]);

                // Get the absolute seat number for the player that played their card
                int cardPlayedSeatNumber = (int)propertiesThatChanged["CardPlayedSeatNumber"];

                // Calculate relative seat number based on the local player's seat
                int relativeSeatNumber = (cardPlayedSeatNumber - seatNumber + 4) % 4;

                // Get the current gameobject that should get the card image.
                GameObject cardGameObject = playedCardObjects[relativeSeatNumber];

                // Set the image for this card
                cardGameObject.GetComponent<RawImage>().texture = cardTextures[card.deckNumber];

                // If this is the first card of the trick, clear out the other cards
                if (trick.CardCount() == 1)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        // Make sure we don't clear out the card that was just played
                        if (i != relativeSeatNumber)
                        {
                            cardGameObject = playedCardObjects[i];
                            cardGameObject.GetComponent<RawImage>().texture = null;
                        }
                    }
                }
            }

            // Handle Score Updates for Team One
            if (propertiesThatChanged.ContainsKey("TeamOneScore"))
            {
                int teamOneScore = (int)propertiesThatChanged["TeamOneScore"];
                teamOneScoreText.text = "Team One Score: " + teamOneScore;
            }

            // Handle Score Updates for Team Two
            if (propertiesThatChanged.ContainsKey("TeamTwoScore"))
            {
                int teamTwoScore = (int)propertiesThatChanged["TeamTwoScore"];
                teamTwoScoreText.text = "Team Two Score: " + teamTwoScore;
            }

            // Handle Trick Updates for Team One
            if (propertiesThatChanged.ContainsKey("TeamOneTricks"))
            {
                int teamOneTricks = (int)propertiesThatChanged["TeamOneTricks"];
                teamOneTricksText.text = "Team One Tricks: " + teamOneTricks;
            }

            // Handle Trick Updates for Team Two
            if (propertiesThatChanged.ContainsKey("TeamTwoTricks"))
            {
                int teamTwoTricks = (int)propertiesThatChanged["TeamTwoTricks"];
                teamTwoTricksText.text = "Team Two Tricks: " + teamTwoTricks;
            }

            // Handle game message updates
            if (propertiesThatChanged.ContainsKey("GameMessage"))
            {
                string gameMessage = (string)propertiesThatChanged["GameMessage"];
                gameMessageText.text = gameMessage;
            }

            base.OnRoomPropertiesUpdate(propertiesThatChanged);
        }
    }
}

