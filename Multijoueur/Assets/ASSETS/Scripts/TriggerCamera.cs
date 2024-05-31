using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class TriggerCamera : MonoBehaviour
{
    [SerializeField] private CameraRotation cameraRotation;
    [SerializeField] private CameraDistance cameraDistance;

    private bool _isInTrigger;
    
    private CinemachineVirtualCamera cam;
    private BoxCollider _box;

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isInTrigger = true;
            PokemonManager.instance.localPlayer.isInCameraTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isInTrigger = false;
            PokemonManager.instance.localPlayer.isInCameraTrigger = false;
        }
    }

    private void Update()
    {
        if (!cam) return;

        if (_isInTrigger && PokemonManager.instance.localPlayer.isInCameraTrigger)
        {
            if(cameraRotation) cameraRotation.AnimationForward(cam);
            if(cameraDistance) cameraDistance.AnimationForward(cam);
        }
        else if(!_isInTrigger && !PokemonManager.instance.localPlayer.isInCameraTrigger)
        {
            if(cameraRotation) cameraRotation.AnimationBackward(cam);
            if(cameraDistance) cameraDistance.AnimationBackward(cam);
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        _box = GetComponent<BoxCollider>();
        if (!_box) return;
        Helpers.DrawBoxCollider(Color.red, transform, _box, 0.2f);
    }
    #endif
}
