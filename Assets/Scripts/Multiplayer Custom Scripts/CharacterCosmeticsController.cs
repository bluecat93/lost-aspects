using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using Mirror;

public class CharacterCosmeticsController : MonoBehaviour
{
    public int currentColorIndex = 0;
    public Material[] playerColors;
    public Image currentColorImage;
    public Text CurrentColorText;

    private void Start()
    {
        currentColorIndex = PlayerPrefs.GetInt("CurrentColorIndex", 0);
        currentColorImage.color = playerColors[currentColorIndex].color;
        CurrentColorText.text = playerColors[currentColorIndex].name;
        GetComponent<LobbyController>().LocalPlayerController.ChangeColor(currentColorIndex);

    }

    public void NextColor()
    {
        currentColorIndex = currentColorIndex < playerColors.Length - 1 ? currentColorIndex + 1 : 0;

        PlayerPrefs.SetInt("currentColorIndex", currentColorIndex);
        currentColorImage.color = playerColors[currentColorIndex].color;
        CurrentColorText.text = playerColors[currentColorIndex].name;

        GetComponent<LobbyController>().LocalPlayerController.ChangeColor(currentColorIndex);
    }

    public void PreviousColor()
    {
        currentColorIndex = currentColorIndex > 0 ? currentColorIndex - 1 : playerColors.Length - 1;

        PlayerPrefs.SetInt("currentColorIndex", currentColorIndex);
        currentColorImage.color = playerColors[currentColorIndex].color;
        CurrentColorText.text = playerColors[currentColorIndex].name;

        GetComponent<LobbyController>().LocalPlayerController.ChangeColor(currentColorIndex);
    }
}
