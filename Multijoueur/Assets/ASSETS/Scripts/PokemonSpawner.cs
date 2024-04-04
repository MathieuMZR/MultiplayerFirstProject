using System;
using System.Collections;
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
        GetComponent<SphereCollider>().isTrigger = true;
        GetComponent<SphereCollider>().radius = detectRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(pokemonSpawnedCount.Value < maxPokemon)
                SpawnPokemon();
        }
    }

    void SpawnPokemon()
    {
        pokemonSpawnedCount.Value++;
        
        var posToInCircle = (Random.insideUnitCircle * spawnRadius);
        var posToSpawn = transform.position + new Vector3(posToInCircle.x, 0, posToInCircle.y);

        var instance = Instantiate(PokemonManager.instance.pokemonPrefab.gameObject, 
            posToSpawn, Quaternion.identity);
        
        var instancePokemonScript = instance.GetComponent<Pokemon>();
        instancePokemonScript.InitiatePokemon(GetRandomPokemon());
        instancePokemonScript.spawnerParent = this;
        
        var instanceNetworkObject = instance.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn();
    }

    Pokemon_SO GetRandomPokemon() => possiblePokemon[Random.Range(0, possiblePokemon.Length)];
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
