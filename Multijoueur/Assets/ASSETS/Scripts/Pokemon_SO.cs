using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SO_Pkmn", menuName = "Pokémon/new Pokémon", order = 1)]
public class Pokemon_SO : ScriptableObject
{
    public List<PokemonVariation> spriteVariations = new List<PokemonVariation>();

    public bool canBeAltForm;

    public string pokemonName;
    public int pokemonID;
    public PokemonRarity pokemonRarity;
    public PokemonType pokemonType;
    
    public PokemonSpeed pokemonSpeed;

    public enum PokemonRarity { Common, Uncommon, Rare, Epic, Legendary }
    public enum PokemonType { Plante, Nourriture, Animal }
    public enum PokemonSpeed { VerySlow, Slow, Normal, Fast, VeryFast }

    public float SpeedByEnum(PokemonSpeed ps)
    {
        var f = 1f;
        switch (ps)
        {
            case PokemonSpeed.VerySlow: 
                f = 2f;
                break;
            case PokemonSpeed.Slow:
                f = 1.5f;
                break;
            case PokemonSpeed.Normal:
                f = 1f;
                break;
            case PokemonSpeed.Fast:
                f = 0.5f;
                break;
            case PokemonSpeed.VeryFast:
                f = 0.25f;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(ps), ps, null);
        }
        return f / 5f;
    }
    
    public float CatchRateByEnum(PokemonRarity pr)
    {
        var cr = 0f;
        switch (pr)
        {
            case PokemonRarity.Common:
                cr = 0.8f;
                break;
            case PokemonRarity.Uncommon:
                cr = 0.65f;
                break;
            case PokemonRarity.Rare:
                cr = 0.4f;
                break;
            case PokemonRarity.Epic:
                cr = 0.2f;
                break;
            case PokemonRarity.Legendary:
                cr = 0.075f;
                break;
            default:
                cr = 0.75f;
                break;
        }
        return cr;
    }
}

[Serializable]
public class PokemonVariation
{
    public Sprite pokemonSpriteVariation;
    public Sprite pokemonSpriteShinyVariation;
    public float variationProbability;
    public string pokemonVariationName;
}
