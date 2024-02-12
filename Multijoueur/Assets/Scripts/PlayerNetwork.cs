using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float linearDragMultiplier;
    [SerializeField] private float linearDragDecelerator;
    [SerializeField] private TextMeshPro displayInfo;
   
    [SerializeField] private Animator animator;
    [SerializeField] private Transform pivotTransform;

    [SerializeField] private GameObject bomb;
    private GameObject lastBombSpawned;
    
    private Vector3 direction;
    private Vector3 directionNotReset;
    private Rigidbody rb;

    public override void OnNetworkSpawn()
    {
        //target only the owner
        if (IsOwner)
        {
            CameraManager.Singleton.InitTarget(transform);
            rb = GetComponent<Rigidbody>();
        }
        
        SetupColorsFromID(OwnerClientId);
        InitDisplayInfos();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            //Debug.Log(OwnerClientId);
            RotateFromInput();
            HandleAnimations();

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (IsHost)
                {
                    SpawnPlayerSphere(OwnerClientId,
                        transform.position + new Vector3(0, 0.5f, 0));
                }
                else
                {
                    SpawnPlayerSphere_ServerRpc(OwnerClientId,
                        transform.position + new Vector3(0, 0.5f, 0));
                }
            }
        }
        else
        {
            
        }
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            Move();
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
        
        rb.AddForce(Helper.IsoConvertVector(direction, CameraManager.Singleton.cam.transform.eulerAngles.y)
                    * (moveSpeed * Time.deltaTime), ForceMode.Impulse);

        rb.drag = linearDragMultiplier * linearDragDecelerator;
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
    private void InitDisplayInfos()
    {
        displayInfo.text = $"Player {OwnerClientId}";
    }

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

    [ServerRpc]
    void SpawnPlayerSphere_ServerRpc(ulong connectionId, Vector3 pos)
    {
        SpawnPlayerSphere(connectionId, pos);
    }
    
    void SpawnPlayerSphere(ulong connectionId, Vector3 pos)
    {
        //int playerIDint = Convert.ToInt32(connectionId);

        lastBombSpawned = Instantiate(bomb, pos, Quaternion.identity);
        
        lastBombSpawned.GetComponent<NetworkObject>().Spawn(true);
        lastBombSpawned.GetComponent<Bomb>().ownerID.Value = Convert.ToInt32(OwnerClientId);
    }

    void SetupColorsFromID(ulong clientID)
    {
        foreach (MeshRenderer r in GetComponentsInChildren<MeshRenderer>())
        {
            r.material.SetColor("_Color", NetworkColors.Singleton.playerColors[clientID]);
        }
    }
}