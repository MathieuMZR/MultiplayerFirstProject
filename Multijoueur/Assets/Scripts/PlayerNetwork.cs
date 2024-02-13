using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float linearDragMultiplier;
    [SerializeField] private float linearDragDecelerator;
    [SerializeField] private TextMeshPro displayInfo;
    [SerializeField] private TextMeshPro nearestObject;
   
    [SerializeField] private Animator animator;
    [SerializeField] private Transform pivotTransform;
    
    [SerializeField] private GameObject mesh;
    [SerializeField] private Transform morphPropParent;
    
    [SerializeField] private GameObject bomb;
    
    public List<GameObject> allAvailableProps = new List<GameObject>();
    public NetworkVariable<GameObject> nearestProp = new NetworkVariable<GameObject>(null, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    private GameObject morphPropGameObject;
    private GameObject lastBombSpawned;
    private Vector3 direction;
    private Vector3 directionNotReset;
    private Rigidbody rb;

    public override void OnNetworkSpawn()
    {
        //target only the owner
        if (IsOwner)
        {
            GetComponents();
            
            CameraManager.Singleton.InitTarget(transform);
        }
        
        SetupColorsFromID(OwnerClientId);
        InitDisplayInfos();
    }

    void GetComponents()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            //Debug.Log(OwnerClientId);
            RotateFromInput();
            HandleAnimations();
            HandleBombInputs();

            if (Input.GetKeyDown(KeyCode.R) && nearestProp is not null)
            {
                if (IsHost)
                {
                    SpawnProp();
                }
                else
                {
                    SpawnProp_ServerRpc();
                }
            }
        }
        
        nearestObject.text = nearestProp is not null ? nearestProp.Value.name : "None";
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            Move();
        }
    }

    public void GetNearestProp()
    {
        if (allAvailableProps.Count > 0)
            nearestProp.Value = allAvailableProps.OrderBy(obj => Vector3.Distance(obj.transform.position, transform.position))
                .ToList()[0];
        else
            nearestProp = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Prop>())
        {
            AddProp(other);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Prop>())
        {
            RemoveProp(other);
        }
    }

    void AddProp(Collider other)
    {
        var prop = other.gameObject;
        if (!allAvailableProps.Contains(prop))
        {
            allAvailableProps.Add(prop);
            GetNearestProp();
        }
    }

    void RemoveProp(Collider other)
    {
        var prop = other.gameObject;
        if (allAvailableProps.Contains(prop))
        {
            allAvailableProps.Remove(prop);
            GetNearestProp();
        }
    }

    #region PropsSpawn
    
    [ServerRpc]
    void SpawnProp_ServerRpc()
    {
        SpawnProp();
    }
    
    void SpawnProp()
    {
        morphPropGameObject = Instantiate(nearestProp.Value.gameObject);
        morphPropGameObject.GetComponent<NetworkObject>().Spawn();
        
        mesh.SetActive(false);
        
        morphPropGameObject.GetComponent<SphereCollider>().enabled = false;
        morphPropGameObject.AddComponent<NetworkFollow>().Init(transform);
    }
    
    #endregion

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
    
    #region Bombs
    
    void HandleBombInputs()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (IsHost)
            {
                SpawnBomb(transform.position + new Vector3(0, 0.5f, 0));
            }
            else
            {
                SpawnBomb_ServerRpc(transform.position + new Vector3(0, 0.5f, 0));
            }
        }
    }

    [ServerRpc]
    void SpawnBomb_ServerRpc(Vector3 pos)
    {
        SpawnBomb(pos);
    }
    
    void SpawnBomb(Vector3 pos)
    {
        lastBombSpawned = Instantiate(bomb, pos, Quaternion.identity);
        
        lastBombSpawned.GetComponent<NetworkObject>().Spawn(true);
        lastBombSpawned.GetComponent<Bomb>().ownerID.Value = Convert.ToInt32(OwnerClientId);
    }

    #endregion
    
    #region Visuals

    void SetupColorsFromID(ulong clientID)
    {
        foreach (MeshRenderer r in GetComponentsInChildren<MeshRenderer>())
        {
            r.material.SetColor("_Color", NetworkColors.Singleton.playerColors[clientID]);
        }
    }
    
    #endregion
}