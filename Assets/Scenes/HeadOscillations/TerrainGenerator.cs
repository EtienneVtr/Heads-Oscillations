using UnityEngine;
using UnityEngine.SceneManagement;

public class TerrainGenerator : MonoBehaviour {
    public Terrain terrain; // Reference to the Terrain component
    public float bumpHeight = 1.0f; // Height of the bumps
    public float bumpSpacing = 10.0f; // Spacing between the bumps
    public float noiseScale = 20.0f; // Scale of the Perlin noise for irregular bumps
    public float noiseHeightMultiplier = 1.0f; // Height multiplier for the Perlin noise
    private float[,] originalHeights; // Store the original terrain heights

    void Start() {
        string sceneName = SceneManager.GetActiveScene().name; // Get the current scene name

        // Store the original terrain heights
        TerrainData terrainData = terrain.terrainData;
        originalHeights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);

        if (sceneName == "FlatGround") {
            // Do nothing for FlatGround
        } else if (sceneName == "LinearBumps") {
            GenerateRegularBumps(); // Generate regular bumps for LinearBumps scene
        } else if (sceneName == "Bumps") {
            GenerateIrregularBumps(); // Generate irregular bumps for Bumps scene
        }
    }

    void OnApplicationQuit() {
        // Restore the original terrain heights when the application quits
        RestoreOriginalTerrain();
    }

    void OnDisable() {
        // Restore the original terrain heights when the script is disabled
        RestoreOriginalTerrain();
    }

    void RestoreOriginalTerrain() {
        if (originalHeights != null) {
            TerrainData terrainData = terrain.terrainData;
            terrainData.SetHeights(0, 0, originalHeights); // Restore the heights
        }
    }

    void GenerateRegularBumps() {
        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;

        float[,] heights = terrainData.GetHeights(0, 0, width, height);

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y += (int)bumpSpacing) {
                // Calculate height using a sinusoidal function to create a half-cylinder shape
                float heightValue = Mathf.Sin((float)x / width * Mathf.PI) * bumpHeight / terrainData.size.y;
                heights[x, y] += heightValue;
            }
        }

        // Smooth the heights
        SmoothHeights(heights, width, height);

        terrainData.SetHeights(0, 0, heights); // Apply the modified heights to the terrain
    }

    void SmoothHeights(float[,] heights, int width, int height) {
        int smoothRadius = 1; // Smoothing radius

        for (int x = smoothRadius; x < width - smoothRadius; x++) {
            for (int y = smoothRadius; y < height - smoothRadius; y++) {
                float totalHeight = 0;
                int count = 0;

                for (int i = -smoothRadius; i <= smoothRadius; i++) {
                    for (int j = -smoothRadius; j <= smoothRadius; j++) {
                        totalHeight += heights[x + i, y + j];
                        count++;
                    }
                }

                heights[x, y] = totalHeight / count; // Average height within the smoothing radius
            }
        }
    }

    void GenerateIrregularBumps() {
        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;

        float[,] heights = terrainData.GetHeights(0, 0, width, height);

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                float xCoord = (float)x / width * noiseScale;
                float yCoord = (float)y / height * noiseScale;
                float noiseValue = Mathf.PerlinNoise(xCoord, yCoord) * noiseHeightMultiplier / terrainData.size.y;

                // Limit height variations
                heights[x, y] += Mathf.Clamp(noiseValue, 0, 0.05f); // Adjust clamping range as needed
            }
        }

        terrainData.SetHeights(0, 0, heights); // Apply the modified heights to the terrain
    }
}
