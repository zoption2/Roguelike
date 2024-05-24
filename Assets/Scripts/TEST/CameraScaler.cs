using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    private Camera _mainCamera;

    public Transform leftBoundaryObject;
    public Transform rightBoundaryObject;
    public Transform topBoundaryObject;
    public Transform bottomBoundaryObject;

    void Start()
    {
        _mainCamera = GetComponent<Camera>();
        FitCameraToScreen();
    }

    void FitCameraToScreen()
    {
        float leftBoundary = leftBoundaryObject.position.x;
        float rightBoundary = rightBoundaryObject.position.x;
        float topBoundary = topBoundaryObject.position.y;
        float bottomBoundary = bottomBoundaryObject.position.y;

        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = topBoundary - bottomBoundary;
        _mainCamera.orthographicSize = cameraHeight / 2;

        float cameraWidth = rightBoundary - leftBoundary;
        float desiredAspect = cameraWidth / cameraHeight;

        if (screenAspect >= desiredAspect)
        {
            _mainCamera.orthographicSize = cameraHeight / 2;
        }
        else
        {
            float differenceInSize = desiredAspect / screenAspect;
            _mainCamera.orthographicSize = cameraHeight / 2 * differenceInSize;
        }

        float cameraPosX = (leftBoundary + rightBoundary) / 2;
        float cameraPosY = (topBoundary + bottomBoundary) / 2;
        _mainCamera.transform.position = new Vector3(cameraPosX, cameraPosY, _mainCamera.transform.position.z);
    }
}
