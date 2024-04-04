using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PokemonManager : NetworkBehaviour
{
    public Pokemon pokemonPrefab;
    
    // 1/256 = 0.00390625f
    public float shinyRate = 0.00390625f;
    
    public static PokemonManager instance;

    private void Awake()
    {
        if (instance) Destroy(gameObject);
        else instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
