using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform target;
    [SerializeField] Vector3 offset;
    public float cameraYRotation;
    public float cameraXRotation;
    [SerializeField] float smoothAmount = 5f;
    [SerializeField] Transform cameraTransform;

    public static CameraManager Singleton;

    private void Awake()
    {
        //Init Singleton
        Singleton = this;
    }

    private void Start()
    {
        cameraTransform.rotation = Quaternion.Euler(cameraXRotation,cameraYRotation,0);
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            cameraTransform.position = 
                Vector3.Slerp(cameraTransform.position, target.position + offset, Time.deltaTime * smoothAmount);
        }
    }

    public void InitTarget(Transform target)
    {
        this.target = target;
    }
}
