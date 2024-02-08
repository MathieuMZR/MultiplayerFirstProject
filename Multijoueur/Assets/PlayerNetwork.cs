using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private TextMeshPro displayInfo;
    [SerializeField] private GameObject spawnGameObject;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform transformPivot;
    
    private GameObject spawnGameObjectInstance;
    private Vector3 direction;
    private Vector3 directionNotReset;

    public override void OnNetworkSpawn()
    {
        GetComponentInChildren<MeshRenderer>().material.color = 
            NetworkColors.Singleton.playerColors[OwnerClientId];
    }

    // Update is called once per frame
    void Update()
    {
        InitDisplayInfos();
        
        //Place the functions that needs authority, after this line of code
        if (!IsOwner) return;
        Move();
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spawnGameObjectInstance = Instantiate(spawnGameObject, transform.position, Quaternion.identity);
            spawnGameObjectInstance.GetComponent<NetworkObject>().Spawn(true);
        }
    }

    private void Move()
    {
        direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        if (direction != Vector3.zero) directionNotReset = direction;
        
        transform.position += direction * (moveSpeed * Time.deltaTime);
        
        animator.SetBool("Move", direction.magnitude > 0.25f);
    }

    private void InitDisplayInfos() => displayInfo.text = $"Player {OwnerClientId + 1}";

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