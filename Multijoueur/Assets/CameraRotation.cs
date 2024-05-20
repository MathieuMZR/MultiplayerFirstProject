using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private bool enabled;
    
    [SerializeField] private Vector3 desiredCameraRotation;
    [SerializeField] private float overallSpeed;
    [SerializeField] private AnimationCurve overallCurve;
    
    private Vector3 _baseRotation;

    private float _timer;

    public void AnimationForward(CinemachineVirtualCamera cam, Vector3 baseRotation)
    {
        if (!enabled) return;
        
        if(!_baseRotation.Equals(baseRotation)) _baseRotation = baseRotation;
        _timer.IncreaseTimerIfPositive(overallSpeed);
        AnimationLerp(cam);
    }
    
    public void AnimationBackward(CinemachineVirtualCamera cam, Vector3 baseRotation)
    {
        if (!enabled) return;
        
        if(!_baseRotation.Equals(baseRotation)) _baseRotation = baseRotation;
        _timer.DecreaseTimerIfPositive(overallSpeed);
        AnimationLerp(cam);
    }

    private void AnimationLerp(CinemachineVirtualCamera cam)
    {
        var rotation = cam.transform.rotation;
        rotation = Quaternion.Lerp(Quaternion.Euler(_baseRotation.x, _baseRotation.y, _baseRotation.z), Quaternion.Euler(desiredCameraRotation.x, desiredCameraRotation.y, desiredCameraRotation.z), 
            overallCurve.Evaluate(_timer));
        cam.transform.rotation = rotation;
    }
}
