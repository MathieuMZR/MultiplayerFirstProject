using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Pokedex : MonoBehaviour
{
    [SerializeField] private PokedexBox pokedexBoxPrefab;
    [SerializeField] private Transform boxContainer;

    private List<PokedexBox> boxes = new List<PokedexBox>();

    private void Start()
    {
        InitPokedex();
    }

    private void InitPokedex()
    {
        foreach (var pkmnSo in PokemonManager.instance.allPokemon)
        {
            var box = Instantiate(pokedexBoxPrefab, boxContainer);
            box.InitiateBox(pkmnSo);
            boxes.Add(box);
        }
    }

    public void RefreshAllBoxes()
    {
        foreach (var pkmnBox in boxes)
        {
            pkmnBox.UpdateEncountersAndCatches();
        }
    }   
}
