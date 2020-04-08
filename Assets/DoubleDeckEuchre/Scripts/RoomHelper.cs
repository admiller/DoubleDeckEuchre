using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class RoomHelper : MonoBehaviour
{
    public static Hashtable GetRoomCustomProperties()
    {
        Hashtable customProperties = PhotonNetwork.CurrentRoom.CustomProperties;

        if (customProperties == null)
        {
            customProperties = new Hashtable();
            PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
        }

        return customProperties;
    }

    public static ArrayList GetTeamOnePlayers()
    {
        Hashtable customProperties = GetRoomCustomProperties();

        return customProperties.ContainsKey("TeamOnePlayers") ? Serializer.DeserializeArrayList((byte[])customProperties["TeamOnePlayers"]) : new ArrayList();
    }

    public static ArrayList GetTeamTwoPlayers()
    {
        Hashtable customProperties = GetRoomCustomProperties();

        return customProperties.ContainsKey("TeamTwoPlayers") ? Serializer.DeserializeArrayList((byte[])customProperties["TeamTwoPlayers"]) : new ArrayList();
    }

    public static string GetPlayerNameBySeatNumber(int seatNumber)
    {
        ArrayList teamOnePlayers = GetTeamOnePlayers();
        ArrayList teamTwoPlayers = GetTeamTwoPlayers();
        Player player = null;

        switch (seatNumber)
        {
            case 0:
                player = PhotonNetwork.CurrentRoom.GetPlayer((int)teamOnePlayers[0]);
                break;

            case 1:
                player = PhotonNetwork.CurrentRoom.GetPlayer((int)teamTwoPlayers[0]);
                break;

            case 2:
                player = PhotonNetwork.CurrentRoom.GetPlayer((int)teamOnePlayers[1]);
                break;

            case 3:
                player = PhotonNetwork.CurrentRoom.GetPlayer((int)teamTwoPlayers[1]);
                break;
        }

        return player.NickName;
    }
}
