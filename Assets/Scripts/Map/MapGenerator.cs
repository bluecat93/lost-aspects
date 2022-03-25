using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        Noisemap,
        ColorMap,
        Mesh
    }

    public DrawMode drawMode;

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public int octave;
    [Range(0, 1)] public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
    public bool autoUpdate;
    public TerrainType[] regions;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(this.mapWidth, this.mapHeight, this.seed, this.noiseScale, this.octave, this.persistance, this.lacunarity, this.offset);
        Color[] colorMap = new Color[this.mapWidth * this.mapHeight];

        for (int y = 0; y < this.mapHeight; y++)
        {
            for (int x = 0; x < this.mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];

                foreach (TerrainType region in this.regions)
                {
                    if (currentHeight <= region.height)
                    {
                        colorMap[y * mapWidth + x] = region.color;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();

        switch (this.drawMode)
        {
            case DrawMode.ColorMap:
                display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, this.mapWidth, this.mapHeight));
                break;
            case DrawMode.Mesh:
                display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap), TextureGenerator.TextureFromColorMap(colorMap, this.mapWidth, this.mapHeight));
                break;
            default:
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
                break;
        }

    }

    private void OnValidate()
    {
        if (this.mapWidth < 1)
            this.mapWidth = 1;
        if (this.mapHeight < 1)
            this.mapHeight = 1;

        if (this.lacunarity < 1)
            this.lacunarity = 1;
        if (this.octave < 0)
            this.octave = 0;
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}