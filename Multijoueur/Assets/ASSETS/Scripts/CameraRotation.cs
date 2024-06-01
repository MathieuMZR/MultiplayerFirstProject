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
    
    private void Start()
    {
        PokemonManager.instance.OnLocalPlayerJoined += () =>
        {
            var rotation = PokemonManager.instance.localPlayer.listener.transform.eulerAngles;
            _baseRotation = rotation;
        };
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
        var rotation = cam.transform.rotation;
        rotation = Quaternion.Lerp(Quaternion.Euler(_baseRotation.x, _baseRotation.y, _baseRotation.z), Quaternion.Euler(desiredCameraRotation.x, desiredCameraRotation.y, desiredCameraRotation.z), 
            overallCurve.Evaluate(_timer));
        cam.transform.rotation = rotation;
    }
}
