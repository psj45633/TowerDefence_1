using UnityEngine;

public class BillboardToCamera : MonoBehaviour
{
    private Camera cam;
    
    void Awake()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (!cam) return;

        transform.rotation = cam.transform.rotation;
    }


}
