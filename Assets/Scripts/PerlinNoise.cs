using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    [SerializeField] int width = 256;
    [SerializeField] int height = 256;
    [SerializeField] float scale = 20f;

    [SerializeField] float offsetX = 100f;
    [SerializeField] float offsetY = 100f;

    [SerializeField] string textureName = "_SurfaceNoise";

    void Start()
    {
        offsetX = Random.Range(0f, 99999f);
        offsetY = Random.Range(0f, 99999f);

        Renderer renderer = GetComponent<MeshRenderer>();
        renderer.material.SetTexture(textureName, GenerateTexture());
    }

    Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);
        
        // Generate Perlin Noise Map
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = CalculateColor(x, y);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    Color CalculateColor(int x, int y)
    {
        float xCoord = (float) x / width * scale + offsetX;
        float yCoord = (float) y / height * scale + offsetY;

        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        return new Color(sample, sample, sample);
    }

}
