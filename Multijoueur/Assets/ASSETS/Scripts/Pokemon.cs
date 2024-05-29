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
    
    [SerializeField] private ParticleSystem[] pokemonParticlesSpawn;

    [Header("Components")]
    [SerializeField] private Transform spritePivot;
    [SerializeField] private Light pokemonLight;
    [SerializeField] private ShinyLight pokemonShinyLight;
    [SerializeField] private AnimationCurve moveRotateCurve;

    private PokemonSounds pokemonSounds;
    private Animator animator;

    [Header("Constants")]
    private const float k_lightShiny = 80f;
    private const float k_lifeTime = 30f;

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
        pokemonParticlesSpawn[(int)pokemonScriptable.pokemonType].Play();
    }

    IEnumerator MoveRoutine()
    {
        yield return new WaitForSeconds(Random.Range(1.25f, 5.5f));

        var posToMove = spawnerParent.transform.position + new Vector3(
            Random.Range(-spawnerParent.behaviorRadius, spawnerParent.behaviorRadius) / 2f, 0, 
            Random.Range(-spawnerParent.behaviorRadius, spawnerParent.behaviorRadius) / 2f);

        var timeToMove = pokemonScriptable.SpeedByEnum(pokemonScriptable.pokemonSpeed) * Vector3.Distance(transform.position, posToMove);
        transform.DOMove(posToMove, timeToMove).SetEase(Ease.Linear);
        transform.DORotate(new Vector3(0, 0, 3), timeToMove / 7f)
            .SetEase(moveRotateCurve).SetLoops(7);
        
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
        yield return new WaitForSeconds(k_lifeTime);
        spawnerParent.DeSpawnPokemon(gameObject.GetComponent<NetworkObject>());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var p = other.GetComponent<PlayerController>();
            
            if (!p.IsOwner) return;
            
            if (p == PokemonManager.instance.localPlayer)
            {
                BattleManager.Instance.StartBattle(pkmnData, p);
                p.EnableInputs(false);
            
                StopAllCoroutines();

                StartCoroutine(nameof(DespawnDelay));
            }
            else
            {
                return;
            }
        }
    }

    private IEnumerator DespawnDelay()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy_Rpc();
    }

    [Rpc(SendTo.Server)]
    private void Destroy_Rpc()
    {
        GetComponent<NetworkObject>().Despawn();
    }
}

[Serializable]
public class TransmitPokemonData
{
    public Pokemon_SO pkmnSo;

    public Sprite sprite;
    
    public bool isShiny;
    public bool isAltForm;

    public PokemonVariation var;

    public PlayerController playerTriggerBattle;

    public TransmitPokemonData(Pokemon_SO so, Sprite sprite, bool shiny, bool altForm, PokemonVariation var)
    {
        pkmnSo = so;
        this.sprite = sprite;
        isShiny = shiny;
        isAltForm = altForm;
        this.var = var;
    }
}