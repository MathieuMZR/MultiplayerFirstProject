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
    
    public Action OnLocalPlayerJoined;

    private void Awake()
    {
        if (instance) Destroy(gameObject);
        else instance = this;
    }

    public void OnPlayerJoin(PlayerController p)
    {
        if (p.IsHost)
        {
            connectedPlayers.Value++;
        }
        
        if (p.IsLocalPlayer)
        {
            localPlayer = p;
            OnLocalPlayerJoined.Invoke();
        }
    }
    
    public void OnPlayerQuit(PlayerController p)
    {
        if (p.IsHost)
        {
            connectedPlayers.Value--;
        }
    }

    public Pokemon_SO FindPokemonFromID(int ID)
    {
        Pokemon_SO pkmn = allPokemon[0];
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
