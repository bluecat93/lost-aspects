using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        Noisemap,
        ColorMap,
        Mesh,
        FalloffMap
    }

    public enum Biomes
    {
        Mountains,
        Forest,
        Grass,
        Desert
    }

    public DrawMode drawMode;
    public Biomes biome;
    // int biomeIndex = Convert.ToInt32(biome);
    public Noise.NormalizeMode normalizeMode;
    public bool isUseFallofMap;
    public const int mapChunkSize = 239;

    [Range(0, 6)] public int editorPreviewLevelOfDetails;

    public bool autoUpdate;
    // public TerrainType[] regions;

    public Biome[] biomes;

    float[,] falloffMap;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    void Awake()
    {
        this.falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
    }

    public void DrawMapInEditor()
    {
        MapData mapData = this.GenerateMapData(Vector2.zero);

        MapDisplay display = FindObjectOfType<MapDisplay>();

        int biomeIndex = Convert.ToInt32(biome);

        switch (this.drawMode)
        {
            case DrawMode.ColorMap:
                display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
                break;
            case DrawMode.Mesh:
                display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, this.biomes[biomeIndex].meshHeightMultiplier, this.biomes[biomeIndex].meshHeightCurve, this.editorPreviewLevelOfDetails), TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
                break;
            case DrawMode.FalloffMap:
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(mapChunkSize)));
                break;
            default:
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
                break;
        }
    }

    // void Start()
    // {
    //     this.DrawMapInEditor();
    // }

    public void RequestMapData(Vector2 center, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(center, callback);
        };

        new Thread(threadStart).Start();
    }

    void MapDataThread(Vector2 center, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(center);
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, lod, callback);
        };

        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
    {
        int biomeIndex = Convert.ToInt32(biome);

        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, this.biomes[biomeIndex].meshHeightMultiplier, this.biomes[biomeIndex].meshHeightCurve, lod);
        lock (this.meshDataThreadInfoQueue)
        {
            this.meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    void Update()
    {
        if (this.mapDataThreadInfoQueue.Count > 0)
        {
            foreach (var item in this.mapDataThreadInfoQueue)
            {
                MapThreadInfo<MapData> threadInfo = this.mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (this.meshDataThreadInfoQueue.Count > 0)
        {
            foreach (var item in this.meshDataThreadInfoQueue)
            {
                MapThreadInfo<MeshData> threadInfo = this.meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    MapData GenerateMapData(Vector2 center)
    {
        int biomeIndex = Convert.ToInt32(biome);
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, this.biomes[biomeIndex].seed, this.biomes[biomeIndex].noiseScale, this.biomes[biomeIndex].octave, this.biomes[biomeIndex].persistance, this.biomes[biomeIndex].lacunarity, center + this.biomes[biomeIndex].offset, normalizeMode);
        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];

        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                if (this.isUseFallofMap)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }
                float currentHeight = noiseMap[x, y];

                foreach (TerrainType region in this.biomes[biomeIndex].terrainTypes)
                {
                    if (currentHeight >= region.height)
                        colorMap[y * mapChunkSize + x] = region.color;
                    else
                        break;
                }

                // foreach (TerrainType region in this.regions)
                // {
                //     if (currentHeight >= region.height)
                //         colorMap[y * mapChunkSize + x] = region.color;
                //     else
                //         break;
                // }
            }
        }

        return new MapData(noiseMap, colorMap);
    }


    private void OnValidate()
    {
        int biomeIndex = Convert.ToInt32(biome);
        if (this.biomes[biomeIndex].lacunarity < 1)
            this.biomes[biomeIndex].lacunarity = 1;
        if (this.biomes[biomeIndex].octave < 0)
            this.biomes[biomeIndex].octave = 0;

        this.falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}

[System.Serializable]
public struct Biome
{
    public string name;
    public float noiseScale;
    public int octave;
    [Range(0, 1)] public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
    public TerrainType[] terrainTypes;
}

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;

    public MapData(float[,] heightMap, Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}

