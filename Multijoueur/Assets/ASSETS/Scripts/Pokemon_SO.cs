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
    public Vector2 extensionMoveMinMax;

    public enum PokemonRarity { Common, Uncommon, Rare, Epic, Legendary }
    public enum PokemonType { Plant, Food, Abstract, Mythical }
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
}

[Serializable]
public class PokemonVariation
{
    public Sprite pokemonSpriteVariation;
    public Sprite pokemonSpriteShinyVariation;
    public float variationProbability;
}
