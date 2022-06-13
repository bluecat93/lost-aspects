using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManagerHandler : MonoBehaviour
{
    public GameObject NetworkManager;
    public static bool IsNetworkManagerActive = false;
    // Start is called before the first frame update
    void Start()
    {
        if (!NetworkManager.activeSelf)
        {
            if (!IsNetworkManagerActive)
            {
                IsNetworkManagerActive = true;
                NetworkManager.SetActive(true);
            }
        }
    }
}
