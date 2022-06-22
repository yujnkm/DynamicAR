using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TopDown : MonoBehaviour
{
    public Camera cam;
    public KeyCode screenshotKey;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
        screenshotKey = KeyCode.R;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(Input.GetKeyDown(screenshotKey))
        {
            Capture();
        }
    }

    public void Capture()
    {
        RenderTexture activeRenderTexture = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;
        cam.Render();
        Texture2D image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = activeRenderTexture;
        byte[] bytes = image.EncodeToPNG();
        Destroy(image);
        File.WriteAllBytes("Assets/Backgrounds/kirby.png", bytes);
    }
}
