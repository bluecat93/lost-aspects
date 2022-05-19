using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class RespawnScript : MonoBehaviour
    {
        [SerializeField] private Transform respawnPoint;

        public void RespawnPlayer(GameObject player)
        {
            player.transform.position = this.respawnPoint.transform.position;
            Physics.SyncTransforms();
        }
    }
}