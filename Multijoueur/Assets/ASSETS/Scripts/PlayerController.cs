using System;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : NetworkBehaviour
{
    public bool allowInputs;
    public bool walkHardCoded;
    public bool isInCameraTrigger;
    
    [SerializeField] private float groundDrag;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float walkSpeed;
    
    public CinemachineVirtualCamera vc;
    public Camera listener;
    
    [SerializeField] private Transform spriteTransform;
    
    [SerializeField] private EmoteWheel emoteWheel;

    private Rigidbody _rb;
    private PlayerEmotes _playerEmotes;
    private Animator _animator;
    
    public Vector3 direction;
    public Vector3 lastDirection;

    private bool _isOnGround;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _playerEmotes = GetComponent<PlayerEmotes>();
        _animator = GetComponent<Animator>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SetupSelfCamera();

        PokemonManager.instance.OnPlayerJoin(this);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        
        PokemonManager.instance.OnPlayerQuit(this);
    }

    public void EnableInputs(bool enable)
    {
        allowInputs = enable;
        GetComponent<Collider>().enabled = enable;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        
        AnimationManagement();
        
        SpriteRotation();
        
        if (!allowInputs)
        {
            if(_rb.velocity.magnitude > 0f) _rb.velocity = Vector3.zero;
            if(Mathf.Abs(direction.magnitude) > 0f && !walkHardCoded) direction = Vector3.zero;
            return;
        }
        
        SetupDirection();
        EmoteManagement();
    }
    
    private void FixedUpdate()
    {
        _isOnGround = Physics.Raycast(transform.position + new Vector3(0,1,0), Vector3.down, 1.25f, groundMask);

        _rb.drag = groundDrag;
        _rb.AddForce(direction * walkSpeed, ForceMode.Impulse);
        _rb.AddForce(Vector3.down * 200f);
    }

    private void SetupDirection()
    {
        direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (direction.magnitude > 0.2f) lastDirection = direction;
    }
    private void SetupSelfCamera()
    {
        if (IsOwner)
        {
            listener.enabled = true;
            vc.Priority = 1;
        }
        else
        {
            vc.Priority = 0;
        }
    }

    #region Animation
    private void SpriteRotation()
    {
        var axis = lastDirection.x > 0 ? 1 : -1;
        spriteTransform.DOScaleX(axis, 0.15f);
    }
    
    private void AnimationManagement()
    {
        _animator.SetBool("IsMoving", direction.magnitude > 0.1f);
    }
    
    #endregion

    #region Emote

    private void EmoteManagement()
    {
        if(!emoteWheel) Helpers.ErrorReferenceInspectorFromGo(gameObject);
        var enablingCondition = Input.GetKey(KeyCode.H);

        emoteWheel.gameObject.SetActive(enablingCondition);

        if (enablingCondition)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    [Rpc(SendTo.Server)]
    public void Emote_Rpc(int index)
    {
        EmoteClient_Rpc(index);
    }

    [Rpc(SendTo.Everyone)]
    private void EmoteClient_Rpc(int index)
    {
        StartCoroutine(_playerEmotes.StartNewEmote(index));
    }
    
    #endregion

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position + new Vector3(0,1,0), Vector3.down * 1.25f);
        Gizmos.DrawRay(transform.position + new Vector3(0,1,0) + transform.forward * 2f + lastDirection,  Vector3.down * 1.25f);
    }
    #endif
}