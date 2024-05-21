using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    public float targetHeightInUnits = 18;

    void Update()
    {
        AdjustCameraSize();
    }

    void AdjustCameraSize()
    {
        Camera camera = GetComponent<Camera>();

        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float targetWidthInUnits = targetHeightInUnits * aspectRatio;

        camera.orthographicSize = targetHeightInUnits / 2.0f;
    }
}
