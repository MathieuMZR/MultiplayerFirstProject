using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceCamera : MonoBehaviour
{
    private Canvas canvas;
    private Camera cam;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        canvas = GetComponent<Canvas>();
        yield return new WaitUntil(() => canvas.worldCamera != null);
        cam = canvas.worldCamera;
    }

    private void Update()
    {
        if (cam) canvas.worldCamera = cam;
    }
}
