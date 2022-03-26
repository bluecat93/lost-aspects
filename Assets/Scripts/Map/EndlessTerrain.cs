using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    const float scale = 0.5f;
    const float viewerMoveThresholdForChunkUpdate = 25f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    public LODInfo[] detailLevels;
    public static float maxViewDst;

    public Transform viewer;
    public Material mapMaterial;

    static MapGenerator mapGenerator;

    public static Vector2 viewerPosition;
    Vector2 viewerPositionOld;
    int chunkSize;
    int chunksVisibleInViewDst;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();

        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        this.chunkSize = MapGenerator.mapChunkSize - 1;
        this.chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / this.chunkSize);

        this.UpdateVisibleChunks();
    }

    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / scale;

        if ((this.viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            this.viewerPositionOld = viewerPosition;

            this.UpdateVisibleChunks();
        }
        this.UpdateVisibleChunks();
    }


    void UpdateVisibleChunks()
    {
        foreach (TerrainChunk chunk in terrainChunksVisibleLastUpdate)
        {
            chunk.SetVisible(false);
        }

        terrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / this.chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / this.chunkSize);

        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, this.chunkSize, this.detailLevels, this.transform, this.mapMaterial));
                }

            }
        }
    }


    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        MeshCollider meshCollider;

        LODInfo[] detailLevels;
        LODMesh[] lodMeshes;
        LODMesh collisionLodMesh;

        MapData mapData;
        bool isMapDataReceived;
        int previousLODIndex = -1;

        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material)
        {
            this.detailLevels = detailLevels;

            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            this.meshObject = new GameObject("Terrain Chunk");
            this.meshRenderer = this.meshObject.AddComponent<MeshRenderer>();
            this.meshFilter = this.meshObject.AddComponent<MeshFilter>();
            this.meshCollider = this.meshObject.AddComponent<MeshCollider>();

            this.meshRenderer.material = material;
            this.meshObject.transform.position = positionV3 * scale;
            this.meshObject.transform.parent = parent;
            this.meshObject.transform.localScale = Vector3.one * scale;

            SetVisible(false);

            this.lodMeshes = new LODMesh[detailLevels.Length];

            for (int i = 0; i < detailLevels.Length; i++)
            {
                this.lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);

                if (this.detailLevels[i].isUseForCollider)
                {
                    this.collisionLodMesh = this.lodMeshes[i];
                }
            }

            mapGenerator.RequestMapData(position, OnMapDataReceived);
        }

        void OnMapDataReceived(MapData mapData)
        {
            this.mapData = mapData;
            this.isMapDataReceived = true;

            Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colorMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
            meshRenderer.material.mainTexture = texture;

            this.UpdateTerrainChunk();
        }

        public void UpdateTerrainChunk()
        {
            if (isMapDataReceived)
            {
                float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool isVisible = viewerDstFromNearestEdge <= maxViewDst;

                if (isVisible)
                {
                    int lodIndex = 0;

                    for (int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (lodIndex != previousLODIndex)
                    {
                        LODMesh lodMesh = lodMeshes[lodIndex];

                        if (lodMesh.isHaveMesh)
                        {
                            previousLODIndex = lodIndex;
                            this.meshFilter.mesh = lodMesh.mesh;

                        }
                        else if (!lodMesh.isRequestedMesh)
                        {
                            lodMesh.RequestMesh(mapData);
                        }
                    }

                    if (lodIndex == 0)
                    {
                        if (this.collisionLodMesh.isHaveMesh)
                            this.meshCollider.sharedMesh = this.collisionLodMesh.mesh;
                        else if (!this.collisionLodMesh.isRequestedMesh)
                            this.collisionLodMesh.RequestMesh(mapData);
                    }

                    terrainChunksVisibleLastUpdate.Add(this);
                }

                SetVisible(isVisible);
            }
        }

        public void SetVisible(bool isVisible)
        {
            meshObject.SetActive(isVisible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }

    class LODMesh
    {
        public Mesh mesh;
        public bool isRequestedMesh;
        public bool isHaveMesh;
        int lod;
        System.Action updateCallBack;

        public LODMesh(int lod, System.Action updateCallBack)
        {
            this.lod = lod;
            this.updateCallBack = updateCallBack;
        }

        void OnMeshDataReceived(MeshData meshData)
        {
            this.mesh = meshData.CreateMesh();
            this.isHaveMesh = true;

            this.updateCallBack();
        }

        public void RequestMesh(MapData mapData)
        {
            this.isRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData, this.lod, OnMeshDataReceived);
        }

    }


    [System.Serializable]
    public struct LODInfo
    {
        public int lod;
        public float visibleDstThreshold;
        public bool isUseForCollider;
    }

}
