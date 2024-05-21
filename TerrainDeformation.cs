using System;
using System.Collections;
using UnityEngine;

public class TerrainDeformation : MonoBehaviour
{
    [SerializeField] Terrain terrain;
    [Range(0.001f, 0.1f)] [SerializeField] float strength;
    [SerializeField] float lowestHeight;
    [SerializeField] int radius;
    [SerializeField] int width, height;

    [SerializeField] Transform player;

    private TerrainData terrainData;
    private int heightmapResolution;
    private float[,] originalHeights;

    void Start()
    {
        terrainData = terrain.terrainData;
        heightmapResolution = terrainData.heightmapResolution;
        originalHeights = terrainData.GetHeights(0, 0, heightmapResolution, heightmapResolution);
    }

    private void OnDisable() 
    {
        terrainData.SetHeights(0, 0, originalHeights);
    }

    public void LowerTerrain(Vector3 worldPosition, float strength, int radius, int width, int height, float playerRotation)
    {
        var brushPosition = GetBrushPosition(worldPosition, radius);
        var brushRadius = GetSafeBrushRadius(brushPosition.x, brushPosition.y, radius);
 
        var heights = terrainData.GetHeights(brushPosition.x, brushPosition.y, brushRadius * 2, brushRadius * 2);
 
        for (var y = 0; y < brushRadius * 2; y++)
        {
            for (var x = 0; x < brushRadius * 2; x++)
            {
                float r1 = (Mathf.Cos(Mathf.Deg2Rad * (360 - playerRotation)) * (x - brushRadius) + Mathf.Sin(Mathf.Deg2Rad * (360 - playerRotation)) * (y - brushRadius)) / width;
                float r2 = (Mathf.Sin(Mathf.Deg2Rad * (360 - playerRotation)) * (x - brushRadius) - Mathf.Cos(Mathf.Deg2Rad * (360 - playerRotation)) * (y - brushRadius)) / height;

                if (Mathf.Pow(r1, 2) + Mathf.Pow(r2, 2) <= 1)
                {
                    heights[y, x] -= strength * Time.deltaTime;
                    if(heights[y, x] <= lowestHeight) return;
                }
            }
        }
 
        terrainData.SetHeightsDelayLOD(brushPosition.x, brushPosition.y, heights);
    }

    private Vector3 WorldToTerrainPosition(Vector3 worldPosition)
    {
        var terrainPosition = worldPosition - terrain.GetPosition();
        var terrainSize = terrainData.size;
        terrainPosition = new Vector3(terrainPosition.x / terrainSize.x, terrainPosition.y / terrainSize.y, terrainPosition.z / terrainSize.z);

        return new Vector3(terrainPosition.x * heightmapResolution, 0, terrainPosition.z * heightmapResolution);
    }

    private Vector2Int GetBrushPosition(Vector3 worldPosition, int radius)
    {
        var terrainPosition = WorldToTerrainPosition(worldPosition);

        return new Vector2Int((int)Mathf.Clamp(terrainPosition.x - radius, 0.0f, heightmapResolution), (int)Mathf.Clamp(terrainPosition.z - radius, 0.0f, heightmapResolution));
    }

    public int GetSafeBrushRadius(int brushX, int brushY, int radius)
    { 
        while(heightmapResolution - (brushX + radius*2) < 0 || heightmapResolution - (brushY + radius*2) < 0) radius--;

        return radius;
    }
}

