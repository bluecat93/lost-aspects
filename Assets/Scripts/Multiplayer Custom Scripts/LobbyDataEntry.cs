using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class LobbyDataEntry : MonoBehaviour
{
    #region Data
    public CSteamID lobbyID;
    public string lobbyName;
    public Text lobbyNameText;
    #endregion

    public void SetLobbyData()
    {
        lobbyNameText.text = lobbyName == string.Empty ? "Empty" : lobbyName;
    }

    // links to button
    public void JoinLobby()
    {
        SteamLobby.Instance.JoinLobby(lobbyID);
    }
}
