using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class MultiplayerMovementControllerTutorial : NetworkBehaviour
{
    public float Speed = 0.1f;
    public GameObject playerModel;
    public string SceneName;

    private void Start()
    {
        playerModel.SetActive(false);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == SceneName)
        {
            // used only once when scene is active
            if (playerModel.activeSelf == false)
            {
                SetPosition();
                playerModel.SetActive(true);
            }
            // used only in places you want that the client controls itself.
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
}
