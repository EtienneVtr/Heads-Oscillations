using UnityEngine;
using UnityEngine.SceneManagement;

public class TerrainGene : MonoBehaviour
{
    public Terrain terrain;
    public float bumpHeight = 1.0f;
    public float bumpSpacing = 10.0f;
    public float noiseScale = 20.0f;
    public float noiseHeightMultiplier = 1.0f;
    private float[,] originalHeights;

    void Start() {
        string sceneName = SceneManager.GetActiveScene().name;

        // Store the original terrain heights
        TerrainData terrainData = terrain.terrainData;
        originalHeights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);

        if (sceneName == "Flat") {
            // Do nothing for FlatGround
        }
        else if (sceneName == "RegularBumps") {
            GenerateRegularBumps();
        }
        else if (sceneName == "IrregularBumps") {
            GenerateIrregularBumps();
        }
    }

    void OnApplicationQuit() {
        // Restore the original terrain heights
        RestoreOriginalTerrain();
    }

    void OnDisable() {
        // Restore the original terrain heights
        RestoreOriginalTerrain();
    }

    void RestoreOriginalTerrain() {
        if (originalHeights != null) {
            TerrainData terrainData = terrain.terrainData;
            terrainData.SetHeights(0, 0, originalHeights);
        }
    }

    void GenerateRegularBumps() {
        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;

        float[,] heights = terrainData.GetHeights(0, 0, width, height);

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y += (int)bumpSpacing) {
                // Calculate the height using a sine function to create a half-cylinder shape
                float heightValue = Mathf.Sin((float)x / width * Mathf.PI) * bumpHeight / terrainData.size.y;
                heights[x, y] = heightValue;
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }

    void GenerateIrregularBumps() {
        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;

        float[,] heights = new float[width, height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                float xCoord = (float)x / width * noiseScale;
                float yCoord = (float)y / height * noiseScale;
                heights[x, y] = Mathf.PerlinNoise(xCoord, yCoord) * noiseHeightMultiplier / terrainData.size.y;
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }
}
