using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCameraDirection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        OrientDisplayInfoToCamera();
    }
    
    private void OrientDisplayInfoToCamera()
    {
        var eulersCam = CameraManager.Singleton.cam.transform.eulerAngles;
        transform.rotation = Quaternion.Euler(eulersCam.x, eulersCam.y,0);
    }
}
