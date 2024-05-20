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

    public void AnimationForward(CinemachineVirtualCamera cam, float baseDistance)
    {
        if (!enabled) return;

        var framingTransposer = cam.GetCinemachineComponent<CinemachineFramingTransposer>();
        
        if(Math.Abs(framingTransposer.m_CameraDistance - desiredCameraDistance) > 0.01f) _baseDistance = baseDistance;
        _timer.IncreaseTimerIfPositive(overallSpeed);
        AnimationLerp(cam);
    }
    
    public void AnimationBackward(CinemachineVirtualCamera cam, float baseDistance)
    {
        if (!enabled) return;
        
        var framingTransposer = cam.GetCinemachineComponent<CinemachineFramingTransposer>();
        
        if(Math.Abs(framingTransposer.m_CameraDistance - desiredCameraDistance) > 0.01f) _baseDistance = baseDistance;
        _timer.DecreaseTimerIfPositive(overallSpeed);
        AnimationLerp(cam);
    }

    private void AnimationLerp(CinemachineVirtualCamera cam)
    {
        var framingTransposer = cam.GetCinemachineComponent<CinemachineFramingTransposer>();
        var distance = framingTransposer.m_CameraDistance;

        distance = Mathf.Lerp(_baseDistance, desiredCameraDistance, overallCurve.Evaluate(_timer));
        framingTransposer.m_CameraDistance = distance;
    }
}
