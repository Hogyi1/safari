using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainController : MonoBehaviour
{
    public Terrain terrain;
    public float adjustRadius = 10f;  // Area around the building to modify
    public float blendStrength = 2f;  // Smooths the transition
    public float restoreSpeed = 2f;   // Speed for restoring terrain

    private Dictionary<Vector3, float[,]> storedHeights = new Dictionary<Vector3, float[,]>();

    public void AdjustTerrainToBuilding(GameObject building)
    {
        TerrainData terrainData = terrain.terrainData;

        Vector3 buildingPos = building.transform.position;
        float buildingBottom = building.GetComponent<Renderer>().bounds.min.y; // Bottom Y position
        float buildingTop = building.GetComponent<Renderer>().bounds.max.y;   // Top of the building

        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;

        Vector3 terrainPosition = buildingPos - terrain.transform.position;
        int xCenter = Mathf.RoundToInt((terrainPosition.x / terrainData.size.x) * heightmapWidth);
        int zCenter = Mathf.RoundToInt((terrainPosition.z / terrainData.size.z) * heightmapHeight);

        int radius = Mathf.RoundToInt((adjustRadius / terrainData.size.x) * heightmapWidth);

        float[,] originalHeights = terrainData.GetHeights(xCenter - radius / 2, zCenter - radius / 2, radius, radius);

        if (!storedHeights.ContainsKey(buildingPos))
        {
            storedHeights[buildingPos] = originalHeights;
        }

        float[,] newHeights = new float[radius, radius];

        for (int i = 0; i < radius; i++)
        {
            for (int j = 0; j < radius; j++)
            {
                // Calculate world position of terrain point
                float worldX = terrain.transform.position.x + ((xCenter - radius / 2 + i) / (float)heightmapWidth) * terrainData.size.x;
                float worldZ = terrain.transform.position.z + ((zCenter - radius / 2 + j) / (float)heightmapHeight) * terrainData.size.z;
                float terrainHeight = terrainData.GetHeight(xCenter - radius / 2 + i, zCenter - radius / 2 + j);
                float worldY = terrainHeight + terrain.transform.position.y;

                // Compute distance from the center to apply smoothing
                float distX = (i - radius / 2) / (float)(radius / 2);
                float distZ = (j - radius / 2) / (float)(radius / 2);
                float distance = Mathf.Sqrt(distX * distX + distZ * distZ);
                float blendFactor = Mathf.Clamp01(1 - Mathf.Pow(distance, blendStrength));

                float targetHeight = terrainHeight;

                // First, ensure terrain is high enough under the building
                if (terrainHeight < buildingBottom)
                {
                    targetHeight = buildingBottom;
                }

                // Now, check if terrain is too high and blocking anything
                if (targetHeight > buildingBottom + 0.2f)
                {
                    targetHeight = buildingBottom + 0.2f; // Ensure clearance for doors/windows
                }



                float normalizedTargetHeight = (targetHeight - terrain.transform.position.y) / terrainData.size.y;
                newHeights[i, j] = Mathf.Lerp(originalHeights[i, j], normalizedTargetHeight, blendFactor);
            }
        }

        terrainData.SetHeights(xCenter - radius / 2, zCenter - radius / 2, newHeights);
    }

    public void RestoreTerrain(GameObject building)
    {
        if (!storedHeights.ContainsKey(building.transform.position))
        {
            Debug.LogWarning("No stored terrain data found for this position.");
            return;
        }

        StartCoroutine(SmoothRestore(building.transform.position));
    }

    private IEnumerator SmoothRestore(Vector3 position)
    {
        TerrainData terrainData = terrain.terrainData;

        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;

        Vector3 terrainPosition = position - terrain.transform.position;
        int xCenter = Mathf.RoundToInt((terrainPosition.x / terrainData.size.x) * heightmapWidth);
        int zCenter = Mathf.RoundToInt((terrainPosition.z / terrainData.size.z) * heightmapHeight);

        float[,] originalHeights = storedHeights[position];
        int radius = originalHeights.GetLength(0);
        float[,] currentHeights = terrainData.GetHeights(xCenter - radius / 2, zCenter - radius / 2, radius, radius);

        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * restoreSpeed;

            float[,] newHeights = new float[radius, radius];

            for (int i = 0; i < radius; i++)
            {
                for (int j = 0; j < radius; j++)
                {
                    newHeights[i, j] = Mathf.Lerp(currentHeights[i, j], originalHeights[i, j], progress);
                }
            }

            terrainData.SetHeights(xCenter - radius / 2, zCenter - radius / 2, newHeights);
            yield return null;
        }

        terrainData.SetHeights(xCenter - radius / 2, zCenter - radius / 2, originalHeights);
        storedHeights.Remove(position);
    }
}
