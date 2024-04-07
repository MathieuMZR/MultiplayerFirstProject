using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class Pokemon : NetworkBehaviour
{
    [Header("Infos")]
    public Pokemon_SO pokemonScriptable;
    public string pokemonName;
    public int pokemonID;
    public Pokemon_SO.PokemonRarity pokemonRarity;

    public bool isShiny;
    public bool isAltForm;

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
        
        StartCoroutine(nameof(MoveRoutine));
        StartCoroutine(nameof(LifeTime));
    }

    public void InitiatePokemon(Pokemon_SO pokemon)
    {
        pokemonScriptable = pokemon;
        
        isShiny = Random.value < PokemonManager.instance.shinyRate;
        
        if(pokemonScriptable.canBeAltForm)
            isAltForm = Random.value < pokemonScriptable.variationProbability;
        
        SpawnAnimation();
        
        animator = GetComponent<Animator>();
        pokemonSounds = GetComponent<PokemonSounds>();
        
        pokemonSprite.sprite = isAltForm ? pokemonScriptable.pokemonSpriteVariation : pokemonScriptable.pokemonSprite;

        if (isShiny)
        {
            pokemonSprite.sprite = isAltForm ? pokemonScriptable.pokemonSpriteShinyVariation : pokemonScriptable.pokemonSpriteShiny;
            
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
        
        pokemonSounds.AppearSound(isShiny);

        pokemonID = pokemonScriptable.pokemonID;
        pokemonRarity = pokemonScriptable.pokemonRarity;
    }

    void SpawnAnimation()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.2f);

        transform.DOJump(transform.position, 0.5f, 1, 0.5f);
        
        if(isShiny) pokemonShinyParticlesSpawn.Play();
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
        
        SpriteRotation((posToMove - transform.position).normalized);
        SetAnimatorWalkPropriety(true, timeToMove);

        yield return new WaitForSeconds(timeToMove);

        SetAnimatorWalkPropriety(false, 0f);

        StartCoroutine(nameof(MoveRoutine));
    }
    
    private void SpriteRotation(Vector3 dir)
    {
        var axis = dir.x > 0 ? 1 : -1;
        pokemonSprite.transform.DOScaleX(axis, 0.15f);
    }

    void SetAnimatorWalkPropriety(bool isWalking, float currentMoveDuration)
    {
        animator.SetBool("isWalking", isWalking);
        animator.speed = isWalking ? Mathf.Lerp(2f, 0.5f, currentMoveDuration / 5f) : 1f;
    }

    IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(15f);
        spawnerParent.DeSpawnPokemon(gameObject.GetComponent<NetworkObject>());
    }
}
