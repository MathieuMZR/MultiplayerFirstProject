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
    [SerializeField] private float desiredCameraDistance;
    
    private bool _isInTrigger;
    
    private CinemachineVirtualCamera cam;
    private BoxCollider _box;
    
    private Vector3 _baseRotation;
    private float _baseDistance;

    private void Start()
    {
        PokemonManager.instance.OnPlayerJoined += () =>
        {
            cam = PokemonManager.instance.localPlayer.vc;
            _baseRotation = cam.transform.rotation.eulerAngles;
            _baseDistance = cam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance;
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isInTrigger = false;
        }
    }

    private void Update()
    {
        if (!cam) return;

        if (_isInTrigger)
        {
            cameraRotation.AnimationForward(cam, _baseRotation);
            cameraDistance.AnimationForward(cam, _baseDistance);
        }
        else
        {
            cameraRotation.AnimationBackward(cam, _baseRotation);
            cameraDistance.AnimationBackward(cam, _baseDistance);
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
