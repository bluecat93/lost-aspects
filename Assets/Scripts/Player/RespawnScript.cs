using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnScript : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform respawnPoint;

    public void RespawnPlayer()
    {
        this.player.transform.position = this.respawnPoint.transform.position;
        Physics.SyncTransforms();
    }
}
