using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class MultiplayerMovementControllerTutorial : NetworkBehaviour
{
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
        float xDirection = Input.GetAxis("Horizontal");
        float zDirection = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(xDirection, 0.0f, zDirection);


        transform.position += moveDirection * Speed;
    }

    public void PlayerCosmeticsSetup()
    {
        PlayerMesh.material = PlayerColors[GetComponent<PlayerObjectController>().playerColor];
    }
}
