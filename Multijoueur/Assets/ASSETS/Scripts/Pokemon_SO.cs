using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SO_Pkmn", menuName = "Pokémon/new Pokémon", order = 1)]
public class Pokemon_SO : ScriptableObject
{
    public Sprite pokemonSprite;
    public Sprite pokemonSpriteVariation;
    public Sprite pokemonSpriteShiny;
    public Sprite pokemonSpriteShinyVariation;

    public bool canBeAltForm;
    public float variationProbability;
    
    public string pokemonName;
    public int pokemonID;
    public PokemonRarity pokemonRarity;
    
    [Range(0.35f, 3f)] public float timeToMove;
    public Vector2 extensionMoveMinMax;

    public enum PokemonRarity { Common, Uncommon, Rare, Epic, Legendary }
}
