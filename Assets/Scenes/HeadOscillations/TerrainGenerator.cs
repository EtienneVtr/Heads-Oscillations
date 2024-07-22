using UnityEngine;
using UnityEngine.SceneManagement;

public class TerrainGenerator : MonoBehaviour
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

        if (sceneName == "FlatGround") {
            // Do nothing for FlatGround
        }
        else if (sceneName == "LinearBumps") {
            GenerateRegularBumps();
        }
        else if (sceneName == "Bumps") {
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
                // Calculer la hauteur en utilisant une fonction sinusoïdale pour créer une forme de demi-cylindre
                float heightValue = Mathf.Sin((float)x / width * Mathf.PI) * bumpHeight / terrainData.size.y;
                heights[x, y] += heightValue;
            }
        }

        // Lissage des hauteurs
        SmoothHeights(heights, width, height);

        terrainData.SetHeights(0, 0, heights);
    }

    void SmoothHeights(float[,] heights, int width, int height) {
        int smoothRadius = 1; // Rayon de lissage

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

                heights[x, y] = totalHeight / count;
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

                // Limiter les variations de hauteur
                heights[x, y] += Mathf.Clamp(noiseValue, 0, 0.05f); // Ajuster la plage de clamping selon les besoins
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }
}
