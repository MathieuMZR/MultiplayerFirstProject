using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Pokemon : NetworkBehaviour
{
    [Header("Infos")]
    public Pokemon_SO pokemonScriptable;
    public NetworkVariable<int> pokemonID = new NetworkVariable<int>();

    public NetworkVariable<bool> isShiny;
    public NetworkVariable<bool> isAltForm;
    
    public NetworkVariable<bool> isMoving;

    private NetworkVariable<float> lastDirectionX = new NetworkVariable<float>(1f);

    [Header("Spawner")]
    public PokemonSpawner spawnerParent;

    [Header("Shiny")]
    [SerializeField] private SpriteRenderer pokemonSprite;
    [SerializeField] private ParticleSystem pokemonShinyParticle;
    [SerializeField] private ParticleSystem pokemonShinyParticlesSpawn;

    [Header("Components")]
    [SerializeField] private Transform spritePivot;
    [SerializeField] private Light pokemonLight;
    [SerializeField] private ShinyLight pokemonShinyLight;

    private PokemonSounds pokemonSounds;
    private Animator animator;

    [Header("Constants")]
    private const float k_lightShiny = 80f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        InitiatePokemon();

        if (!IsOwner) return;

        StartCoroutine(nameof(MoveRoutine));
        StartCoroutine(nameof(LifeTime));
    }
    
    private void Update()
    {
        SpriteRotation(lastDirectionX.Value);
        SetAnimatorWalkPropriety();
    }

    private void InitiatePokemon()
    {
        pokemonScriptable = PokemonManager.instance.allPokemon[pokemonID.Value];
        
        if (IsHost)
        {
            isShiny.Value = Random.value < PokemonManager.instance.shinyRate;

            if (pokemonScriptable.canBeAltForm)
                isAltForm.Value = Random.value < pokemonScriptable.variationProbability;
        }
        
        if(IsOwner) PokemonManager.instance.localPlayer.UpdatePokemonData(pokemonScriptable, true, false, isShiny.Value);
        
        SpawnAnimation();
        
        animator = GetComponent<Animator>();
        pokemonSounds = GetComponent<PokemonSounds>();
        
        pokemonSprite.sprite = isAltForm.Value ? pokemonScriptable.pokemonSpriteVariation : pokemonScriptable.pokemonSprite;

        if (isShiny.Value)
        {
            pokemonSprite.sprite = isAltForm .Value? pokemonScriptable.pokemonSpriteShinyVariation : pokemonScriptable.pokemonSpriteShiny;
            
            pokemonShinyParticle.gameObject.SetActive(true);
            pokemonLight.intensity = k_lightShiny;
            pokemonLight.spotAngle *= 2f;
            
            pokemonSprite.material.SetInt("_isShiny", 1);
        }
        else
        {
            pokemonShinyLight.enabled = false;
            pokemonLight.enabled = false;
        }
        
        pokemonSounds.AppearSound(isShiny.Value);
    }

    void SpawnAnimation()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.2f);

        transform.DOJump(transform.position, 0.5f, 1, 0.5f);
        
        if(isShiny.Value) pokemonShinyParticlesSpawn.Play();
    }

    IEnumerator MoveRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        
        var posToMove = spawnerParent.transform.position +
                        new Vector3(Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y) *
                        Random.Range(pokemonScriptable.extensionMoveMinMax.x,
                            pokemonScriptable.extensionMoveMinMax.y);
        
        var timeToMove = pokemonScriptable.timeToMove / 5f * Vector3.Distance(transform.position, posToMove);
        transform.DOMove(posToMove, timeToMove).SetEase(Ease.Linear);
        
        if (IsHost) lastDirectionX.Value = (posToMove - transform.position).normalized.x;

        if (IsHost) isMoving.Value = true;

        yield return new WaitForSeconds(timeToMove);
        
        if (IsHost) isMoving.Value = false;

        StartCoroutine(nameof(MoveRoutine));
    }
    
    private void SpriteRotation(float dirX)
    {
        var axis = dirX > 0 ? 1 : -1;
        pokemonSprite.transform.DOScaleX(axis, 0.15f);
    }

    void SetAnimatorWalkPropriety()
    {
        animator.SetBool("isWalking", isMoving.Value);
    }

    IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(15f);
        spawnerParent.DeSpawnPokemon(gameObject.GetComponent<NetworkObject>());
    }
}