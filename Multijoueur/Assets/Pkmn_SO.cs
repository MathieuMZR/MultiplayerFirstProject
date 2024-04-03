using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Pkmn", menuName = "Pokémon/new Pokémon", order = 1)]
public class Pkmn_SO : ScriptableObject
{
    public Sprite pkmnSprite;
    public string pkmnName;
    public int pkmnID;
    public PkmnRarity pkmnRarity;
    
    public float moveSpeed;

    public enum PkmnRarity { Common, Uncommon, Rare, Epic, Legendary }
}
