using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour
{
    #region Player Data
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIdNumber;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool Ready;


    #region Cosmetics
    [SyncVar(hook = nameof(SendPlayerColor))] public int playerColor;
    #endregion
    #endregion

    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Start()
    {
        // makes the game object that this code is assigned to, to not be destroyed when switching scenes.
        DontDestroyOnLoad(this.gameObject);
    }

    private void PlayerReadyUpdate(bool oldValue, bool NewValue)
    {
        if (isServer)
        {
            this.Ready = NewValue;
        }
        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    [Command]
    private void CmdSetPlayerReady()
    {
        this.PlayerReadyUpdate(this.Ready, !this.Ready);
    }

    public void ChangeReady()
    {
        if (hasAuthority)
        {
            CmdSetPlayerReady();
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = Finals.LOCAL_GAME_PLAYER;
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string PlayerName)
    {
        this.PlayerNameUpdate(this.PlayerName, PlayerName);
    }

    public void PlayerNameUpdate(string OldValue, string NewValue)
    {
        // Host
        if (isServer)
        {
            this.PlayerName = NewValue;
        }
        // Client
        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    #region Start Game
    public void CanStartGame(string SceneName)
    {
        // we dont want everyone to be able to run this, only the host needs it.
        if (hasAuthority)
        {
            CmdCanStartGame(SceneName);
        }

    }

    [Command]
    public void CmdCanStartGame(string SceneName)
    {
        Manager.StartGame(SceneName);
    }
    #endregion

    #region Cosmetics

    public void ChangeColor(int newValue)
    {
        if (hasAuthority)
        {
            CmdUpdatePlayerColor(newValue);
        }
    }

    [Command]
    public void CmdUpdatePlayerColor(int newValue)
    {
        SendPlayerColor(playerColor, newValue);
    }

    public void SendPlayerColor(int oldValue, int newValue)
    {
        // host
        if (isServer)
        {
            playerColor = newValue;
        }
        // client, the other check is to reduce latency
        if (isClient && (oldValue != newValue))
        {
            UpdateColor(newValue);
        }
    }
    void UpdateColor(int message)
    {
        playerColor = message;
    }


    #endregion

}
