using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRendrer;
    public void DrawTexture(Texture2D texture)
    {

        textureRendrer.sharedMaterial.mainTexture = texture;
        textureRendrer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }
}