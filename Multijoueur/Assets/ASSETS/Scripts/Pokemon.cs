using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class Pokemon : NetworkBehaviour
{
    public Pokemon_SO pokemonScriptable;
    public string pokemonName;
    public int pokemonID;
    public Pokemon_SO.PokemonRarity pokemonRarity;

    public PokemonSpawner spawnerParent;
    
    [SerializeField] private SpriteRenderer pokemonSprite;
    [SerializeField] private ParticleSystem pokemonShinyParticles;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        StartCoroutine(nameof(MoveRoutine));
    }

    public void InitiatePokemon(Pokemon_SO pokemon)
    {
        pokemonScriptable = pokemon;

        pokemonSprite.sprite = pokemonScriptable.pokemonSprite;
        if (Random.value < PokemonManager.instance.shinyRate)
        {
            pokemonSprite.sprite = pokemonScriptable.pokemonSpriteShiny;
            pokemonShinyParticles.Play();
        }
        
        pokemonID = pokemonScriptable.pokemonID;
        pokemonRarity = pokemonScriptable.pokemonRarity;
    }

    IEnumerator MoveRoutine()
    {
        var posToMove = spawnerParent.transform.position +
                        new Vector3(Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y) *
                        Random.Range(pokemonScriptable.extensionMoveMinMax.x, 
                            pokemonScriptable.extensionMoveMinMax.y);

        var timeToMove = pokemonScriptable.moveSpeed *
                         Vector3.Distance(transform.position, posToMove);
            
        transform.DOMove(posToMove, timeToMove).SetEase(Ease.Linear);

        yield return new WaitForSeconds(timeToMove);

        StartCoroutine(nameof(MoveRoutine));
    }
}
