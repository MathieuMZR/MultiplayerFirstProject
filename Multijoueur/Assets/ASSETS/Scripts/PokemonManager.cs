using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PokemonManager : NetworkBehaviour
{
    public Pokemon pokemonPrefab;
    public NetworkVariable<int> connectedPlayers = new NetworkVariable<int>();

    public Pokemon_SO[] allPokemon;
    
    public PlayerController localPlayer;
    
    // 1/256 = 0.00390625f
    public float shinyRate = 0.00390625f;
    
    public static PokemonManager instance;

    private void Awake()
    {
        if (instance) Destroy(gameObject);
        else instance = this;
    }

    public void OnPlayerJoin()
    {
        connectedPlayers.Value++;
        
        localPlayer = NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId]
            .PlayerObject.GetComponent<PlayerController>();
    }
    
    public void OnPlayerQuit()
    {
        connectedPlayers.Value--;
    }
}
