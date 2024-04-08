using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class PokemonSpawner : NetworkBehaviour
{
    [SerializeField] private int[] possiblePokemonIDs;
    [SerializeField] private float spawnRadius;
    [SerializeField] private float detectRadius;
    [SerializeField] private int maxPokemon;

    public NetworkVariable<int> pokemonSpawnedCount = new NetworkVariable<int>();

    private IEnumerator Start()
    {
        //Spawning wait to at least one player to connect.
        yield return new WaitUntil(() => PokemonManager.instance.connectedPlayers.Value > 0);
        
        //Check if the spawner is owner, then spawn only on the server before replicate.
        if (!IsHost) yield break;
        
        //Call RPC to replicate to server / clients.
        StartCoroutine(nameof(SpawnLoop));
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
    private void SpawnPokemon()
    {
        pokemonSpawnedCount.Value++;
        
        var posToSpawn = transform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), 
            transform.position.y, Random.Range(-spawnRadius, spawnRadius));
        
        var instance = Instantiate(PokemonManager.instance.pokemonPrefab.gameObject, 
            posToSpawn, Quaternion.identity);
        
        var instancePokemonScript = instance.GetComponent<Pokemon>();
        instancePokemonScript.pokemonID.Value = GetRandomPokemonID();
        instancePokemonScript.spawnerParent = this;
        
        var instanceNetworkObject = instance.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn();
    }

    int GetRandomPokemonID() => possiblePokemonIDs[Random.Range(0, possiblePokemonIDs.Length)];

    public void DeSpawnPokemon(NetworkObject obj)
    {
        if (!IsOwner) return;
        
        pokemonSpawnedCount.Value--;
        obj.Despawn();
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, Vector3.one * spawnRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, Vector3.one * detectRadius);
    }
}
