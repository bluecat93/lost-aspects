using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRendrer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void DrawTexture(Texture2D texture)
    {
        this.textureRendrer.sharedMaterial.mainTexture = texture;
        this.textureRendrer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        this.meshFilter.sharedMesh = meshData.CreateMesh();
        this.meshRenderer.sharedMaterial.mainTexture = texture;
    }
}