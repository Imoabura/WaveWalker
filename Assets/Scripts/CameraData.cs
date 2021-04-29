using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraData : MonoBehaviour
{
    [SerializeField] Shader normalsShader;
    [SerializeField] DepthTextureMode textureMode;

    Camera mainCam;
    Camera normalsCam;

    RenderTexture renderTexture;

    // Start is called before the first frame update
    void Awake()
    {
        mainCam = GetComponent<Camera>();
        mainCam.depthTextureMode = textureMode;
    }

    void Start()
    {
        renderTexture = new RenderTexture(mainCam.pixelWidth, mainCam.pixelHeight, 24);
        Shader.SetGlobalTexture("_CameraNormalsTexture", renderTexture);

        GameObject copy = new GameObject("Normals Camera");
        copy.transform.SetParent(transform);
        normalsCam = copy.AddComponent<Camera>();
        normalsCam.CopyFrom(mainCam);
        normalsCam.targetTexture = renderTexture;
        normalsCam.SetReplacementShader(normalsShader, "RenderType");
        normalsCam.depth = mainCam.depth - 1;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
