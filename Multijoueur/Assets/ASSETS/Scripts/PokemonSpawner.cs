using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class PokemonSpawner : NetworkBehaviour
{
    [SerializeField] private Pokemon_SO[] possiblePokemon;
    [SerializeField] private float spawnRadius;
    [SerializeField] private float detectRadius;
    [SerializeField] private int maxPokemon;

    public NetworkVariable<int> pokemonSpawnedCount = new NetworkVariable<int>();

    private void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
        GetComponent<BoxCollider>().size = Vector3.one * detectRadius;

        StartCoroutine(nameof(SpawnLoop));
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(pokemonSpawnedCount.Value < maxPokemon)
                SpawnPokemon();
        }
    }*/

    void SpawnPokemon()
    {
        pokemonSpawnedCount.Value++;
        
        var posToSpawn = transform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), 
                transform.position.y, Random.Range(-spawnRadius, spawnRadius));
        
        var instance = Instantiate(PokemonManager.instance.pokemonPrefab.gameObject, 
            posToSpawn, Quaternion.identity);
        
        var instancePokemonScript = instance.GetComponent<Pokemon>();
        instancePokemonScript.InitiatePokemon(GetRandomPokemon());
        instancePokemonScript.spawnerParent = this;
        
        var instanceNetworkObject = instance.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn();
    }

    Pokemon_SO GetRandomPokemon() => possiblePokemon[Random.Range(0, possiblePokemon.Length)];

    public void DeSpawnPokemon(NetworkObject obj)
    {
        pokemonSpawnedCount.Value--;
        obj.Despawn();
    }

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(1.5f);
        if (pokemonSpawnedCount.Value < maxPokemon)
        {
            SpawnPokemon();
        }
        StartCoroutine(nameof(SpawnLoop));
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, Vector3.one * spawnRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, Vector3.one * detectRadius);
    }
}
