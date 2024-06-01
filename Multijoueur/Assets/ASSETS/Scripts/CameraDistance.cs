using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraDistance : MonoBehaviour
{
    [SerializeField] private bool enabled;
    
    [SerializeField] private float desiredCameraDistance;
    [SerializeField] private float overallSpeed;
    [SerializeField] private AnimationCurve overallCurve;
    
    private float _baseDistance;

    private float _timer;

    private void Start()
    {
        
    }

    public void AnimationForward(CinemachineVirtualCamera cam)
    {
        if (!enabled) return;

        _timer.IncreaseTimerIfPositive(overallSpeed);
        AnimationLerp(cam);
    }
    
    public void AnimationBackward(CinemachineVirtualCamera cam)
    {
        if (!enabled) return;
        
        _timer.DecreaseTimerIfPositive(overallSpeed);
        AnimationLerp(cam);
    }

    private void AnimationLerp(CinemachineVirtualCamera cam)
    {
        var framingTransposer = cam.GetCinemachineComponent<CinemachineFramingTransposer>();
        
        var distance = Mathf.Lerp(_baseDistance, desiredCameraDistance, overallCurve.Evaluate(_timer));
        framingTransposer.m_CameraDistance = distance;
    }
}
