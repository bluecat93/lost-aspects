using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

namespace Object
{
    public class ItemSpawn : NetworkBehaviour
    {
        [Server]
        public void SpawnItem(GameObject itemPrefab, Vector3 location, Quaternion rotation)
        {
            Debug.Log("I AM HERE");
            // create object on server only
            // GameObject obj = Instantiate(itemPrefab, location, rotation);

            // make server spawn object for all clients and gives the object a network id.
            // NetworkServer.Spawn(obj);
        }
    }
}