using UnityEngine;

public class DistortionNoise : MonoBehaviour
{
    [SerializeField] int width = 256;
    [SerializeField] int height = 256;
    [SerializeField] float scale = 20f;

    [SerializeField] string textureName = "_DistortionTexture";

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);

        Renderer renderer = GetComponent<MeshRenderer>();
        renderer.material.SetTexture(textureName, GenerateTexture());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);

        // Generate Perlin Noise Map
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = CalculateColor();
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    Color CalculateColor()
    {
        return new Color(Random.value, Random.value, 0);
    }
}
