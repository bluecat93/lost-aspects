using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;

public class CustomNetworkManager : NetworkManager
{
    [Header("add header")]
    [Tooltip("The player object controller prefab")]
    [SerializeField] private PlayerObjectController GamePlayerPrefab;
    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            // instantiate player controller
            PlayerObjectController GamePlayerInstance = Instantiate(GamePlayerPrefab);

            SteamLobby sl = new SteamLobby();

            // player data
            GamePlayerInstance.ConnectionID = conn.connectionId;
            GamePlayerInstance.PlayerIdNumber = GamePlayers.Count + 1;
            GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)sl.Instance.CurrentLobbyID, GamePlayers.Count);

            // add the player for every single client connected 
            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
        }
    }

    public void StartGame(string SceneName)
    {
        ServerChangeScene(SceneName);
    }

}
