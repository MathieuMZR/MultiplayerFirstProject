using System;
using System.Collections.Generic;
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
    [SerializeField] private CanvasGroup pokedexGroup;
    
    [Header("Pokemon")] public List<PokemonPokedex> pokemonDatas = new List<PokemonPokedex>();
    
    private Rigidbody _rb;
    private PlayerEmotes _playerEmotes;
    private Animator _animator;
    
    private Vector3 direction;
    private Vector3 lastDirection;

    private bool pokedexOpenned;
    
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

        InitPokedex();
        
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

        ManageOpenPokedex();
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

    public void ManageOpenPokedex()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            pokedexOpenned = !pokedexOpenned;
            
            pokedexGroup.alpha = pokedexOpenned ? 1f : 0f;
            pokedexGroup.interactable = pokedexOpenned;
            pokedexGroup.blocksRaycasts = pokedexOpenned;
        }
    }
    
    private void InitPokedex()
    {
        foreach (var pkmnSo in PokemonManager.instance.allPokemon)
        {
            pokemonDatas.Add(new PokemonPokedex(pkmnSo));
        }
    }

    public void UpdatePokemonData(Pokemon_SO pkmn, bool incrementEnc, bool incrementCatch, bool isShiny)
    {
        foreach (var data in pokemonDatas)
        {
            if (pkmn == data.pokemonScriptable)
            {
                if (incrementEnc) data.encounters++;
                if (incrementCatch) data.catches++;
                
                if(incrementEnc && isShiny) data.encountersShiny++;
                if(incrementCatch && isShiny) data.catchesShiny++;

                pokedexGroup.GetComponent<Pokedex>().RefreshAllBoxes();
            }
        }
    }
}


[Serializable]
public class PokemonPokedex
{
    public Pokemon_SO pokemonScriptable;
    
    public int encounters;
    public int encountersVar;
    public int encountersShiny;
    public int encountersVarShiny;
    
    public int catches;
    public int catchesVar;
    public int catchesShiny;
    public int catchesVarShiny;

    public PokemonPokedex(Pokemon_SO so)
    {
        pokemonScriptable = so;
    }
}