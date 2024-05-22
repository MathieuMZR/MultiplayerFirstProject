using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : GenericSingletonClass<BattleManager>
{
    [Header("Components")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Animator _animator;

    [SerializeField] private List<string> displays = new List<string>();

    [Header("Pokemon")]
    [SerializeField] private SpriteRenderer pkmnSprite;
    [SerializeField] private Image fade;

    private TransmitPokemonData lastPkmn;
    
    public float FadeAmount {
        get { return fade.material.GetFloat("_Size"); }
        set { fade.material.SetFloat("_Size", value); }
    }

    public void StartBattle(TransmitPokemonData encounterPkmn)
    {
        lastPkmn = encounterPkmn;
        
        _animator.SetTrigger("StartBattle");
        MusicManager.Instance.BattleMusic();
        
        pkmnSprite.sprite = lastPkmn.sprite;
    }

    public void GenerateEncounterMessage(int ID)
    {
        if(displays[ID].Contains("<PKMN>")) 
            displays[ID] = displays[ID].Replace("<PKMN>", lastPkmn.pkmnSo.pokemonName);
        
        TextChanger.Instance.GenerateTextBubble(canvas.transform, 
            $"{displays[ID]}");
    }
}