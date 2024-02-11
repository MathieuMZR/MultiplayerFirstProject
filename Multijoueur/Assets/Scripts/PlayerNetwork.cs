using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private TextMeshPro displayInfo;
   
    [SerializeField] private Animator animator;
    [SerializeField] private Transform pivotTransform;

    [SerializeField] private GameObject spawn;
    private GameObject spawnGameObjectInstance;
    
    private Vector3 direction;
    private Vector3 directionNotReset;
    
    //private NetworkVariable<int> ID = new (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        //target only the owner
        if (IsOwner)
        {
            CameraManager.Singleton.InitTarget(transform);
        }
        
        SetupColorsFromID(OwnerClientId);

        InitDisplayInfos();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"ID : {OwnerClientId}");
        
        if (IsOwner)
        {
            //Debug.Log(OwnerClientId);
            RotateFromInput();
            Move();
            HandleAnimations();
        }
        else
        {
            
        }
        
        if (Input.GetKeyDown(KeyCode.F) && IsLocalPlayer)
        {
            SpawnPlayerSphere_ServerRpc(GetComponent<NetworkObject>().OwnerClientId, 
                transform.position + new Vector3(0,0.5f,0));
        }
    }
    
    #region Movements

    /// <summary>
    /// Move the player to the current input direction
    /// </summary>
    private void Move()
    {
        //Set Directions
        var currentInputValues = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        direction = Vector3.Lerp(direction, currentInputValues, 10f * Time.deltaTime);
        if (direction != Vector3.zero) directionNotReset = direction;
        
        
        //Move the player by modifying position
        transform.position += Helper.IsoConvertVector(direction, CameraManager.Singleton.cam.transform.eulerAngles.y)
                              * (moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Make the player rotate whenever he's changing direction
    /// </summary>
    void RotateFromInput()
    {
        if (directionNotReset.magnitude < 0.1f) return;
        
        Quaternion toRotation = Quaternion.LookRotation(directionNotReset, Vector3.up) * 
                                Quaternion.Euler(0, CameraManager.Singleton.cam.transform.eulerAngles.y, 0);

        pivotTransform.localRotation = Quaternion.Slerp(pivotTransform.localRotation, toRotation,
            25 * Time.deltaTime);
    }
    
    #endregion

    #region Name Tag / Infos
    
    /// <summary>
    /// Initialize NameTag infos at the spawn of the player
    /// </summary>
    private void InitDisplayInfos() => displayInfo.text = $"Player {OwnerClientId}";

    #endregion

    #region Animations
    
    /// <summary>
    /// Handle all the player animations, from variables and all
    /// </summary>
    void HandleAnimations()
    {
        //Set the animator plays the animations from bool parameter
        animator.SetBool("Move", direction.magnitude > 0.5f);
    }
    
    #endregion

    [ServerRpc(RequireOwnership = false)]
    void SpawnPlayerSphere_ServerRpc(ulong connectionId, Vector3 pos)
    {
        SpawnPlayerSphere_ClientRpc(connectionId, pos);
    }
    
    [ClientRpc]
    void SpawnPlayerSphere_ClientRpc(ulong connectionId, Vector3 pos)
    {
        Color playerColor = NetworkColors.Singleton.playerColors[Convert.ToInt32(connectionId)];

        GameObject sphere = Instantiate(spawn, pos, Quaternion.identity);
        sphere.GetComponent<NetworkObject>().Spawn(true); // Spawn on the network
        
        foreach (MeshRenderer r in sphere.GetComponentsInChildren<MeshRenderer>())
        {
            r.material.SetColor("_Color", playerColor);
        }
    }

    void SetupColorsFromID(ulong clientID)
    {
        foreach (MeshRenderer r in GetComponentsInChildren<MeshRenderer>())
        {
            r.material.SetColor("_Color", NetworkColors.Singleton.playerColors[clientID]);
        }
    }
}