using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomChanger : MonoBehaviour
{
    public Vector3 newCameraPoz;
    public Vector3 newPlayerPoz;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main.GetComponent<Camera>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.transform.position += newPlayerPoz;
            cam.transform.position += newCameraPoz;
        }
    }
}
