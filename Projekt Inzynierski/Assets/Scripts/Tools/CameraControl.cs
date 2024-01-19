using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Camera fullscreenCamera;
    public Camera windowedCamera;

    void Start()
    {
        fullscreenCamera.rect = new Rect(0, 0, 1, 1);
        windowedCamera.rect = new Rect(0.7f, 0.6f, 0.4f, 0.6f);
    }
}
