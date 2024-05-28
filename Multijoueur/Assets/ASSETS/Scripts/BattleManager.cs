using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BattleManager : GenericSingletonClass<BattleManager>
{
    [Header("Components")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject fadeFloatRef;
    [SerializeField] private ParticleSystem[] particleSystems;

    [Header("PkmnInfos")] 
    [SerializeField]
    private TextMeshProUGUI pkmnName, pkmnForm, pkmnType, pkmnEncounters, pkmnCatchs;
    [SerializeField] private GameObject shinySparklesText;
    [SerializeField] private Image typeColor;

    [SerializeField] private List<string> displays = new List<string>();

    [Header("Pokemon")]
    [SerializeField] private SpriteRenderer pkmnSprite;
    [SerializeField] private Image fade;

    private TransmitPokemonData lastPkmn;
    
    public float FadeAmount {
        set => fade.material.SetFloat("_Size", value);
    }

    private void Start()
    {
        Material mat = Instantiate(fade.material);
        fade.material = mat;
    }

    public void StartBattle(TransmitPokemonData encounterPkmn)
    {
        lastPkmn = encounterPkmn;
        
        Debug.Log("Battle Started");
        
        _animator.SetTrigger("StartBattle");
        MusicManager.Instance.BattleMusic();
        
        InitAllVisuals();
    }

    private void InitAllVisuals()
    {
        pkmnSprite.sprite = lastPkmn.sprite;

        pkmnName.text = lastPkmn.pkmnSo.pokemonName;
        pkmnName.color = lastPkmn.isShiny ? RefColors.ShinyDarker : RefColors.BasicBlack;

        pkmnForm.text = "Forme " + (lastPkmn.var.pokemonVariationName != "" ? lastPkmn.var.pokemonVariationName : "Normale");
        pkmnType.text = Enum.GetName(typeof(Pokemon_SO.PokemonType), lastPkmn.pkmnSo.pokemonType);
        
        foreach (TextDataFromType td in TextChanger.Instance.typeDatas)
        {
            if (td.type == lastPkmn.pkmnSo.pokemonType)
            {
                typeColor.color = td.color;
                break;
            }
        }
        
        pkmnEncounters.text =
            $"Rencontrés : {PokemonManager.instance.localPlayer.GetComponent<Pokedex>().pokedexDatas[lastPkmn.pkmnSo.pokemonID].Encounters}";
        
        pkmnCatchs.text =
            $"Attrapés : {PokemonManager.instance.localPlayer.GetComponent<Pokedex>().pokedexDatas[lastPkmn.pkmnSo.pokemonID].Catch}";
    }

    public void SpawnShinySparkles()
    {
        if (!lastPkmn.isShiny) return;
            
        particleSystems[0].Play();
        particleSystems[1].Play();
        shinySparklesText.SetActive(true);
    }
    
    public void SpawnGrassSparkles()
    {
        particleSystems[2].Play();
    }

    public void GenerateEncounterMessage(int ID)
    {
        if(displays[ID].Contains("<PKMN>")) 
            displays[ID] = displays[ID].Replace("<PKMN>", lastPkmn.pkmnSo.pokemonName);
        
        TextChanger.Instance.GenerateTextBubble(canvas.transform, 
            $"{displays[ID]}");
    }

    private void Update()
    {
        //Need to be between 0 and 500
        FadeAmount = fadeFloatRef.transform.localPosition.x;
    }

    
    public void Test()
    {
        Debug.Log("Animation Event");
    }
}