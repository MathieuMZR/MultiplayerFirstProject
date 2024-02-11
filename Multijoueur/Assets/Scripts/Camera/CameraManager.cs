using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform target;
    private CinemachineVirtualCamera cineCam;
    public Camera cam;

    public static CameraManager Singleton;

    private void Awake()
    {
        //Init Singleton
        Singleton = this;
    }

    private void Start()
    {
        cineCam = FindObjectOfType<CinemachineVirtualCamera>();
    }

    public void InitTarget(Transform target)
    {
        this.target = target;
        cineCam.m_Follow = target;
    }
}
