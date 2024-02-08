using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private TextMeshPro displayInfo;
    [SerializeField] private GameObject spawnGameObject;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform pivotTransform;
    
    private GameObject spawnGameObjectInstance;
    private Vector3 direction;
    private Vector3 directionNotReset;

    public override void OnNetworkSpawn()
    {
        GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex", 
            NetworkColors.Singleton.playerTextures[OwnerClientId]);

        //target only the owner
        if (IsOwner)
        {
            CameraManager.Singleton.InitTarget(transform);
        }
        else
        {
            
        }
        
        InitDisplayInfos();
        
        displayInfo.transform.rotation = 
            Quaternion.Euler(CameraManager.Singleton.cameraYRotation,
                CameraManager.Singleton.cameraYRotation,0);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            //Debug.Log(OwnerClientId);
            RotateFromInput();
            Move();
        }
        else
        {
            
        }
    }

    private void Move()
    {
        //Set Directions
        var currentInputValues = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        direction = Vector3.Lerp(direction, currentInputValues, 10f * Time.deltaTime);
        if (direction.magnitude > 0.2f) directionNotReset = direction;
        
        //Move the player by modifying position
        transform.position += Helper.IsoConvertVector(direction, CameraManager.Singleton.cameraYRotation)
                              * (moveSpeed * Time.deltaTime);
        
        //Set the animator plays the animations from bool parameter
        animator.SetBool("Move", direction.magnitude > 0.25f);
    }

    void RotateFromInput()
    {
        Quaternion toRotation = Quaternion.LookRotation(directionNotReset, Vector3.up) * 
                                Quaternion.Euler(0, CameraManager.Singleton.cameraYRotation, 0);

        pivotTransform.localRotation = Quaternion.Slerp(pivotTransform.localRotation, toRotation,
            25 * Time.deltaTime);
    }

    private void InitDisplayInfos() => displayInfo.text = $"Player {OwnerClientId}";

    //-----------------------DEBUG-----------------------------
    
    [ServerRpc]
    void TestServerRpc(ServerRpcParams serverRpcParams)
    {
        Debug.Log("Message Received by Client " + serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    void TestClientRpc()
    {
        Debug.Log("Message on Client Received");
    }
}