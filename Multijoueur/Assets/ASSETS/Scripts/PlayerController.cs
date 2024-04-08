using Cinemachine;
using DG.Tweening;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float groundDrag;
    [SerializeField] private float walkSpeed;
    
    [SerializeField] private CinemachineVirtualCamera vc;
    [SerializeField] private AudioListener listener;
    
    [SerializeField] private Transform spriteTransform;
    
    [SerializeField] private EmoteWheel emoteWheel;
    
    private Rigidbody _rb;
    private PlayerEmotes _playerEmotes;
    private Animator _animator;
    
    private Vector3 direction;
    private Vector3 lastDirection;
    
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
        
        if (!IsHost) return;
        PokemonManager.instance.OnPlayerJoin();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        
        if (!IsHost) return;
        PokemonManager.instance.OnPlayerQuit();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        SpriteRotation();
        
        SetupDirection();

        EmoteManagement();
        AnimationManagement();
    }
    
    private void FixedUpdate()
    {
        _rb.drag = groundDrag;
        _rb.AddForce(direction * walkSpeed, ForceMode.Impulse);
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
}
