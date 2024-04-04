using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Pkmn", menuName = "Pokémon/new Pokémon", order = 1)]
public class Pokemon_SO : ScriptableObject
{
    public Sprite pokemonSprite;
    public Sprite pokemonSpriteShiny;
    public string pokemonName;
    public int pokemonID;
    public PokemonRarity pokemonRarity;
    
    public float moveSpeed;
    public Vector2 extensionMoveMinMax;

    public enum PokemonRarity { Common, Uncommon, Rare, Epic, Legendary }
}
