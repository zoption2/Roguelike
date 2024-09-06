using Cinemachine;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface ICameraManager
{
    public CinemachineVirtualCamera CreateVirtualCamera(Transform target, string name);
    public void SimulateCollisionEffect(Vector3 collisionDirection);
    public void SetMainCamera(CinemachineVirtualCamera virtualCamera);
    public Task WaitForCameraToReachTarget(Transform target);
}

public class CameraManager : ICameraManager
{
    private List<CinemachineVirtualCamera> _allVirtualCameras = new List<CinemachineVirtualCamera>();

    public CinemachineVirtualCamera CreateVirtualCamera(Transform target, string name)
    {
        string cameraName = name + " Camera";
        GameObject virtualCameraObject = new GameObject(cameraName);
        virtualCameraObject.transform.parent = target;
        CinemachineVirtualCamera virtualCamera = virtualCameraObject.AddComponent<CinemachineVirtualCamera>();

        virtualCamera.Follow = target;
        virtualCamera.LookAt = target;

        CinemachineTransposer transposer = virtualCamera.AddCinemachineComponent<CinemachineTransposer>();
        transposer.m_FollowOffset = new Vector3(0, 20, 0);
        transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
        transposer.m_XDamping = 0;
        transposer.m_YDamping = 0;
        transposer.m_ZDamping = 0;

        CinemachineComposer composer = virtualCamera.AddCinemachineComponent<CinemachineComposer>();
        composer.m_TrackedObjectOffset = new Vector3(0, 0, 0);
        composer.m_LookaheadTime = 0;
        composer.m_LookaheadSmoothing = 0;

        _allVirtualCameras.Add(virtualCamera);

        return virtualCamera;
    }

    public void SetMainCamera(CinemachineVirtualCamera virtualCamera)
    {
        if (virtualCamera != null)
        {
            foreach (var cam in _allVirtualCameras)
            {
                cam.Priority = 0;
            }
            virtualCamera.Priority = 10;
        }
    }

    public async Task WaitForCameraToReachTarget(Transform target)
    {
        while (Vector2.Distance(new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.z),
                                new Vector2(target.position.x, target.position.z)) > 0.1f)
        {
            await Task.Yield();
        }
    }

    public  void SimulateCollisionEffect(Vector3 collisionDirection)
    {
        //foreach (var virtualCamera in _allVirtualCameras)
        //{
        //    var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        //    if (transposer != null)
        //    {
        //        Vector3 originalOffset = transposer.m_FollowOffset;
        //        Vector3 collisionOffset = collisionDirection.normalized * 2f;

        //        transposer.m_FollowOffset += collisionOffset;

        //        await Task.Delay(100);

        //        float elapsedTime = 0f;
        //        float duration = 0.5f;

        //        while (elapsedTime < duration)
        //        {
        //            transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, originalOffset, elapsedTime / duration);
        //            elapsedTime += Time.deltaTime;
        //            await Task.Yield();
        //        }

        //        transposer.m_FollowOffset = originalOffset;
        //    }
        //}
    }

}
