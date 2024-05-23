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
    public TransmitPokemonData pkmnData;
    
    public NetworkVariable<int> pokemonID = new NetworkVariable<int>();
    public NetworkVariable<int> pokemonVariationID = new NetworkVariable<int>();

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
        pokemonScriptable = PokemonManager.instance.FindPokemonFromID(pokemonID.Value);
        
        if (IsHost)
        {
            isShiny.Value = Random.value < PokemonManager.instance.shinyRate;

            if (pokemonScriptable.canBeAltForm)
            {
                pokemonVariationID.Value =
                    pokemonScriptable.spriteVariations.IndexOf(
                        WeightedPokemonVariationSelector.GetRandomItem(pokemonScriptable.spriteVariations));
                
                isAltForm.Value = Random.value < pokemonScriptable.spriteVariations[pokemonVariationID.Value].variationProbability;
            }
        }

        SpawnAnimation();
        
        animator = GetComponent<Animator>();
        pokemonSounds = GetComponent<PokemonSounds>();

        pokemonSprite.sprite = pokemonScriptable.spriteVariations[pokemonVariationID.Value].pokemonSpriteVariation;

        if (isShiny.Value)
        {
            pokemonSprite.sprite =
                pokemonScriptable.spriteVariations[pokemonVariationID.Value].pokemonSpriteShinyVariation;
            
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

        pkmnData = new TransmitPokemonData(pokemonScriptable, pokemonSprite.sprite, isShiny.Value, isAltForm.Value, 
            pokemonScriptable.spriteVariations[pokemonVariationID.Value]); 
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
        
        var timeToMove = pokemonScriptable.SpeedByEnum(pokemonScriptable.pokemonSpeed) * Vector3.Distance(transform.position, posToMove);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var p = new PlayerController();
            
            if (other.GetComponent<PlayerController>() == PokemonManager.instance.localPlayer)
            {
                p = PokemonManager.instance.localPlayer;
            }
            else
            {
                return;
            }
            
            if (p.IsOwner)
            {
                BattleManager.Instance.StartBattle(pkmnData);
                p.EnableInputs(false);
                
                StopAllCoroutines();

                StartCoroutine(nameof(DespawnDelay));
            }
        }
    }

    private IEnumerator DespawnDelay()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy_Rpc();
    }

    [Rpc(SendTo.Everyone)]
    private void Destroy_Rpc()
    {
        GetComponent<NetworkObject>().Despawn();
    }
}

public class TransmitPokemonData
{
    public Pokemon_SO pkmnSo;

    public Sprite sprite;
    
    public bool isShiny;
    public bool isAltForm;

    public PokemonVariation var;

    public TransmitPokemonData(Pokemon_SO so, Sprite sprite, bool shiny, bool altForm, PokemonVariation var)
    {
        pkmnSo = so;
        this.sprite = sprite;
        isShiny = shiny;
        isAltForm = altForm;
        this.var = var;
    }
}