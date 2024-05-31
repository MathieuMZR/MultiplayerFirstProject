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
using Random = UnityEngine.Random;

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

    public TransmitPokemonData lastPkmn;
    
    public float FadeAmount {
        set => fade.material.SetFloat("_Size", value);
    }

    private void Start()
    {
        Material mat = Instantiate(fade.material);
        fade.material = mat;
    }

    public void StartBattle(TransmitPokemonData encounterPkmn, PlayerController p)
    {
        lastPkmn = encounterPkmn;
        lastPkmn.playerTriggerBattle = p;
        
        var pkDex = lastPkmn.playerTriggerBattle.GetComponent<Pokedex>().pokedexDatas;
        foreach (PokedexData pxd in pkDex)
        {
            if (pxd.Pokemon == lastPkmn.pkmnSo)
            {
                pxd.Encounters++;
                break;
            }
        }
        
        CameraTarget.Instance.SwitchTransform(1, 1f);

        _animator.SetTrigger("StartBattle");
        MusicManager.Instance.BattleMusic();
        
        InitAllVisuals();
        
        Canvas.ForceUpdateCanvases();
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

    public void CatchPokemon()
    {
        if (!lastPkmn.playerTriggerBattle.IsOwner) return;
        
        if (Random.value < lastPkmn.pkmnSo.CatchRateByEnum(lastPkmn.pkmnSo.pokemonRarity))
        {
            var pkDex = lastPkmn.playerTriggerBattle.GetComponent<Pokedex>().pokedexDatas;
            foreach (PokedexData pxd in pkDex)
            {
                if (pxd.Pokemon == lastPkmn.pkmnSo)
                {
                    pxd.Catch += 1;
                    
                    _animator.SetTrigger("EndBattle");
                    MusicManager.Instance.GrassLandMusic();

                    lastPkmn.playerTriggerBattle.EnableInputs(true);
                    CameraTarget.Instance.SwitchTransform(0, 1f);
                    
                    particleSystems[0].Stop();
                    particleSystems[1].Stop();
                    
                    return;
                }
            }
        }
    }

    public void FleePokemon()
    {
        if (!lastPkmn.playerTriggerBattle.IsOwner) return;
        
        particleSystems[0].Stop();
        particleSystems[1].Stop();
        
        _animator.SetTrigger("EndBattle");
        MusicManager.Instance.GrassLandMusic();

        lastPkmn.playerTriggerBattle.EnableInputs(true, 2f);
        CameraTarget.Instance.SwitchTransform(0, 1f);
    }
}