using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : GenericSingletonClass<CameraTarget>
{
    public Transform playerPosition;
    [SerializeField] private Transform battlePosition;
    
    [SerializeField] private Vector3 playerPosOffset;
    [SerializeField] private Vector3 playerRot;
    [SerializeField] private Vector3 battleRot;
    
    [SerializeField] private float smoothAmount = 15f;

    private int indexCam;
    
    private void Update()
    {
        UpdatePosition();
    }

    void UpdatePosition()
    {
        var speed = Time.deltaTime * smoothAmount;
        switch (indexCam)
        {
            case 0 :
                if (playerPosition == null) return;
                transform.position = Vector3.Lerp(transform.position, playerPosition.position + playerPosOffset, speed);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(playerRot), speed);
                break;
            
            case 1 :
                if (battlePosition == null) return;
                transform.position = Vector3.Lerp(transform.position, battlePosition.position, speed);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(battleRot), speed);
                break;
        }
    }

    public void SwitchTransform(int i = 0, float delay = 0f)
    {
        StartCoroutine(DelayedSwitchTransform(i, delay));
    }

    IEnumerator DelayedSwitchTransform(int i = 0, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        indexCam = i;
    }
}
