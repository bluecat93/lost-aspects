using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class MultiplayerMovementControllerTutorial : NetworkBehaviour
{

    // Notes:
    // [Command] - is for a CLIENT telling the SERVER to run this method.
    // [ClientRpc] - is for a SERVER telling ALL CLIENTS to run this method.
    // [Server] - means the code can only be run on the SERVER.
    // [SyncVar] - forces the SERVER value of the variable to ALL CLIENTS
    // hasAuthority - boolean that is set to true if the current client has the authority on that object (meaning true if its the current player character)


    public float Speed = 0.1f;
    public GameObject PlayerModel;

    public MeshRenderer PlayerMesh;
    public Material[] PlayerColors;

    public string SceneName;

    private void Start()
    {
        PlayerModel.SetActive(false);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == SceneName)
        {
            // Used only once when scene is active
            if (PlayerModel.activeSelf == false)
            {
                PlayerModel.SetActive(true);
                PlayerCosmeticsSetup();
            }
            // Sets the initial position of the player object
            if (transform.position == Vector3.zero)
            {
                SetPosition();
            }
            // Used only in places you want that the client controls itself.
            if (hasAuthority)
            {
                Movement();
            }
        }
    }

    // help to set up starting position.
    public void SetPosition()
    {
        transform.position = new Vector3(Random.Range(-5f, 5f), 0.8f, Random.Range(-15f, 7f));

    }

    public void Movement()
    {
        float xDirection = Input.GetAxis(Finals.HORIZONTAL_MOVEMENT);
        float zDirection = Input.GetAxis(Finals.VERTICAL_MOVEMENT);

        Vector3 moveDirection = new Vector3(xDirection, 0.0f, zDirection);


        transform.position += moveDirection * Speed;
    }

    public void PlayerCosmeticsSetup()
    {
        PlayerMesh.material = PlayerColors[GetComponent<PlayerObjectController>().playerColor];
    }
}
