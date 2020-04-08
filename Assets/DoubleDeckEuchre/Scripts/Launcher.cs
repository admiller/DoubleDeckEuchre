using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace ADM.DoubleDeckEuchre
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private GameObject controlPanel;

        [SerializeField]
        private Text feedbackText;

        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        bool isConnecting;

        string gameVersion = "1";

        [Space(10)]
        [Header("Custom Variables")]
        public InputField playerNameField;
        public InputField roomNameField;

        [Space(5)]
        public Text playerStatus;
        public Text connectionStatus;

        [Space(5)]
        public GameObject roomJoinUI;
        public GameObject buttonLoadArena;
        public GameObject buttonJoinRoom;
        public GameObject teamSelectUI;
        public Text teamOneListText;
        public Text teamTwoListText;

        string playerName = "";
        string roomName = "";

        EventSystem eventSystem;

        void Start()
        {
            PlayerPrefs.DeleteAll();
                        
            roomJoinUI.SetActive(false);
            buttonLoadArena.SetActive(false);
            teamSelectUI.SetActive(false);

            eventSystem = EventSystem.current;

            // See if we are already connected to a room and if so, leave it
            if (PhotonNetwork.IsConnected
            && PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
                playerStatus.text = "A player disconnected.  Returned to main menu.";
            }
            ConnectToPhoton();
        }

        void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public void SetPlayerName(string name)
        {
            playerName = name;
        }

        public void SetRoomName(string name)
        {
            roomName = name;
        }     

        void ConnectToPhoton()
        {
            connectionStatus.text = "Connecting...";
            PhotonNetwork.GameVersion = gameVersion; 
            PhotonNetwork.ConnectUsingSettings(); 
        }

        public void JoinRoom()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName; 
                RoomOptions roomOptions = new RoomOptions(); 
                TypedLobby typedLobby = new TypedLobby(roomName, LobbyType.Default); 
                PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, typedLobby); 
            }
        }

        public void LoadArena()
        {
            ArrayList teamOnePlayers = RoomHelper.GetTeamOnePlayers();
            ArrayList teamTwoPlayers = RoomHelper.GetTeamTwoPlayers();

            // Make sure each team has 2 players
            if (teamOnePlayers.Count == 2
            && teamTwoPlayers.Count == 2)
            {

                // Set the player properties for each player
                Hashtable ht;
                Player player;

                if (teamOnePlayers.Count > 0)
                {

                    ht = new Hashtable();
                    ht.Add("SeatNumber", 0);
                    player = PhotonNetwork.CurrentRoom.GetPlayer((int)teamOnePlayers[0]);
                    player.SetCustomProperties(ht);

                    if (teamOnePlayers.Count > 1)
                    {
                        ht = new Hashtable();
                        ht.Add("SeatNumber", 2);
                        player = PhotonNetwork.CurrentRoom.GetPlayer((int)teamOnePlayers[1]);
                        player.SetCustomProperties(ht);
                    }
                }


                if (teamTwoPlayers.Count > 0)
                {
                    ht = new Hashtable();
                    ht.Add("SeatNumber", 1);
                    player = PhotonNetwork.CurrentRoom.GetPlayer((int)teamTwoPlayers[0]);
                    player.SetCustomProperties(ht);

                    if (teamTwoPlayers.Count > 1)
                    {
                        ht = new Hashtable();
                        ht.Add("SeatNumber", 3);
                        player = PhotonNetwork.CurrentRoom.GetPlayer((int)teamTwoPlayers[1]);
                        player.SetCustomProperties(ht);
                    }
                }
                PhotonNetwork.LoadLevel("CardTable");
            }
            else
            {
                playerStatus.text = "There must be exactly 2 players on each team!";
            }
        }


        public override void OnConnected()
        {
            // 1
            base.OnConnected();
            // 2
            connectionStatus.text = "Connected to Photon!";
            connectionStatus.color = Color.green;
            roomJoinUI.SetActive(true);
            buttonLoadArena.SetActive(false);

            eventSystem.SetSelectedGameObject(playerNameField.gameObject, new BaseEventData(eventSystem));
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            isConnecting = false;
            controlPanel.SetActive(true);
        }

        public override void OnJoinedRoom()
        {
            // Joined the room
            if (PhotonNetwork.IsMasterClient)
            {
                buttonLoadArena.SetActive(true);
                buttonJoinRoom.SetActive(false);
                playerStatus.text = "You are Lobby Leader";
            }
            else
            {
                playerStatus.text = "Connected to Lobby";
            }

            // Hide the Join Room button
            buttonJoinRoom.SetActive(false);

            // Get the room's custom properties
            Hashtable customProperties = RoomHelper.GetRoomCustomProperties();

            // Get the list of team members, or instantiate if they do not exist
            ArrayList teamOnePlayers = RoomHelper.GetTeamOnePlayers();
            ArrayList teamTwoPlayers = RoomHelper.GetTeamTwoPlayers();

            // Add user to team one by default
            teamOnePlayers.Add(PhotonNetwork.LocalPlayer.ActorNumber);

            // Add lists to hashtable
            customProperties["TeamOnePlayers"] = Serializer.SerializeArrayList(teamOnePlayers);
            customProperties["TeamTwoPlayers"] = Serializer.SerializeArrayList(teamTwoPlayers);

            teamSelectUI.SetActive(true);

            // Push the lists back to the server
            PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.ContainsKey("TeamOnePlayers")
            ||  propertiesThatChanged.ContainsKey("TeamTwoPlayers"))
            {
                UpdateUserLists();
            }

            base.OnRoomPropertiesUpdate(propertiesThatChanged);
        }

        /// <summary>
        /// Updates the team one and two lists on the current UI
        /// </summary>
        public void UpdateUserLists()
        {
            Hashtable customProperties = RoomHelper.GetRoomCustomProperties();

            // Look to see if any users are on team one
            if (customProperties.ContainsKey("TeamOnePlayers"))
            {
                ArrayList teamOnePlayers = RoomHelper.GetTeamOnePlayers();

                string playerList = "";

                // Build the string of players on team one
                foreach (int actorNumber in teamOnePlayers)
                {
                    // Get the player in the room based on the actornumber, which is what is in the ArrayList
                    Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);

                    // Add a \n if there is more than one player on the team
                    if (!string.IsNullOrEmpty(playerList))
                    {
                        playerList += "\n";
                    }

                    playerList += player.NickName;
                }

                teamOneListText.text = playerList;
            }

            // Look to see if any users are on team two
            if (customProperties.ContainsKey("TeamTwoPlayers"))
            {
                ArrayList teamtwoPlayers = RoomHelper.GetTeamTwoPlayers();

                string playerList = "";

                // Build the string of players on team two
                foreach (int actorNumber in teamtwoPlayers)
                {
                    // Get the player in the room based on the actornumber, which is what is in the ArrayList
                    Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);

                    // Add a \n if there is more than one player on the team
                    if (!string.IsNullOrEmpty(playerList))
                    {
                        playerList += "\n";
                    }

                    playerList += player.NickName;
                }

                teamTwoListText.text = playerList;
            }

        }

        /// <summary>
        /// Puts the current player on team one
        /// </summary>
        public void JoinTeamOneOnClicked()
        {
            // Get the current team lists
            ArrayList teamOnePlayers = RoomHelper.GetTeamOnePlayers();
            ArrayList teamTwoPlayers = RoomHelper.GetTeamTwoPlayers();

            // Check to make sure the current player is not already on team one
            if (!teamOnePlayers.Contains(PhotonNetwork.LocalPlayer.ActorNumber))
            {
                teamOnePlayers.Add(PhotonNetwork.LocalPlayer.ActorNumber);
            }

            // Remove from Team Two, if possible
            teamTwoPlayers.Remove(PhotonNetwork.LocalPlayer.ActorNumber);

            // Create hashtable with just the teamlists in it
            Hashtable ht = new Hashtable();
            ht["TeamOnePlayers"] = Serializer.SerializeArrayList(teamOnePlayers);
            ht["TeamTwoPlayers"] = Serializer.SerializeArrayList(teamTwoPlayers);

            // Push the lists back to the server
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
        }

        /// <summary>
        /// Puts the current player on team two
        /// </summary>
        public void JoinTeamTwoOnClicked()
        {
            // Get the current team lists
            ArrayList teamOnePlayers = RoomHelper.GetTeamOnePlayers();
            ArrayList teamTwoPlayers = RoomHelper.GetTeamTwoPlayers();

            // Check to make sure the current player is not already on team two
            if (!teamTwoPlayers.Contains(PhotonNetwork.LocalPlayer.ActorNumber))
            {
                teamTwoPlayers.Add(PhotonNetwork.LocalPlayer.ActorNumber);
            }

            // Remove from Team One, if possible
            teamOnePlayers.Remove(PhotonNetwork.LocalPlayer.ActorNumber);

            // Create hashtable with just the teamlists in it
            Hashtable ht = new Hashtable();
            ht["TeamOnePlayers"] = Serializer.SerializeArrayList(teamOnePlayers);
            ht["TeamTwoPlayers"] = Serializer.SerializeArrayList(teamTwoPlayers);

            // Push the lists back to the server
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
        }

        // Update Method
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (eventSystem.currentSelectedGameObject.GetComponent<InputField>() != playerNameField)
                {
                    eventSystem.SetSelectedGameObject(playerNameField.gameObject, new BaseEventData(eventSystem));
                }
                else
                {
                    eventSystem.SetSelectedGameObject(roomNameField.gameObject, new BaseEventData(eventSystem));
                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (roomNameField.text != "")
                {
                    JoinRoom();
                }
            }
        }
    }
}
