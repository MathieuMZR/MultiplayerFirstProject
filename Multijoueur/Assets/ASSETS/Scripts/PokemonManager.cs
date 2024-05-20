using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
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

    public Action OnPlayerJoined;

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
        
        OnPlayerJoined.Invoke();
    }
    
    public void OnPlayerQuit()
    {
        connectedPlayers.Value--;
    }

    public Pokemon_SO FindPokemonFromID(int ID)
    {
        Pokemon_SO pkmn = null;
        foreach (var p in allPokemon)
        {
            if (p.pokemonID == ID)
            {
                pkmn = p;
                break;
            }
        }
        return pkmn;
    }
}
