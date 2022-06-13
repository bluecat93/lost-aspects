using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class LobbiesListManager : MonoBehaviour
{
    public static LobbiesListManager instance;
    #region Lobbies List Variables
    // the empty gameobject we created in the scene
    public GameObject lobbiesMenu;
    // the item prefab we created
    public GameObject lobbyDataItemPrefab;
    // the place where we create all these prefabs
    public GameObject lobbyListContent;

    #endregion

    public GameObject lobbiesButton, hostButton;

    public List<GameObject> listOfLobbies = new List<GameObject>();


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void GetListOfLobbies()
    {
        lobbiesButton.SetActive(false);
        hostButton.SetActive(false);

        lobbiesMenu.SetActive(true);

        SteamLobby.Instance.GetLobbiesList();
    }

    public void BackToMenu()
    {
        lobbiesMenu.SetActive(false);

        lobbiesButton.SetActive(true);
        hostButton.SetActive(true);

        DestroyLobbies();
    }

    public void DisplayLobbies(List<CSteamID> lobbyIDs, LobbyDataUpdate_t result)
    {
        foreach (CSteamID lobbyID in lobbyIDs)
        {
            if (lobbyID.m_SteamID == result.m_ulSteamIDLobby)
            {
                GameObject createdItem = Instantiate(lobbyDataItemPrefab);

                createdItem.GetComponent<LobbyDataEntry>().lobbyID = (CSteamID)lobbyID.m_SteamID;

                createdItem.GetComponent<LobbyDataEntry>().lobbyName = SteamMatchmaking.GetLobbyData((CSteamID)lobbyID.m_SteamID, "name");

                createdItem.GetComponent<LobbyDataEntry>().SetLobbyData();

                createdItem.transform.SetParent(lobbyListContent.transform);
                createdItem.transform.localScale = Vector3.one;

                listOfLobbies.Add(createdItem);
            }
        }
    }

    public void DestroyLobbies()
    {
        foreach (GameObject lobbyItem in listOfLobbies)
        {
            Destroy(lobbyItem);
        }
        listOfLobbies.Clear();
    }




}
