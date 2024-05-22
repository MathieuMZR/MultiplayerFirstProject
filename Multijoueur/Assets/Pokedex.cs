using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Pokedex : NetworkBehaviour
{
    public List<PokedexData> pokedexDatas = new List<PokedexData>();

    private void Awake()
    {
        PokemonManager.instance.OnLocalPlayerJoined += InitPokedex;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void InitPokedex()
    {
        foreach (Pokemon_SO pkmn in PokemonManager.instance.allPokemon)
            pokedexDatas.Add(new PokedexData(pkmn, 0, 0));
    }

    [Rpc(SendTo.Everyone)]
    public void UpdatePokedex_Rpc(int pokemonID, ulong ownerID)
    {
        if (ownerID == NetworkManager.LocalClientId)
        {
            
        }
    }
}

[Serializable]
public class PokedexData
{
    public Pokemon_SO Pokemon;
    public int Encounters;
    public int Catch;

    public PokedexData(Pokemon_SO pkmn, int ec, int ca)
    {
        Pokemon = pkmn;
        Encounters = ec;
        Catch = ca;
    }
}
