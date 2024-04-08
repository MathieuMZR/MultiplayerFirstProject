using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PokedexBox : MonoBehaviour
{
    [SerializeField] private Image pokemonSprite;
    [SerializeField] private Image pokemonSpriteShiny;
    [SerializeField] private Image pokemonSpriteVar;
    [SerializeField] private Image pokemonSpriteVarShiny;
    [SerializeField] private Image pokemonShinyStar;
    
    [SerializeField] private TextMeshProUGUI pokemonName;
    [SerializeField] private TextMeshProUGUI pokemonEncounters;
    [SerializeField] private TextMeshProUGUI pokemonCatch;

    private int currentFormView;
    private int maxFormView;
    
    private Pokemon_SO pokemonScriptable;

    public void InitiateBox(Pokemon_SO pokemonSo)
    {
        pokemonScriptable = pokemonSo;

        pokemonSprite.sprite = pokemonScriptable.pokemonSprite;
        pokemonSpriteShiny.sprite = pokemonScriptable.pokemonSpriteShiny;
        
        if(pokemonScriptable.pokemonSpriteVariation) pokemonSpriteVar.sprite = pokemonScriptable.pokemonSpriteVariation;
        if(pokemonScriptable.pokemonSpriteShinyVariation) pokemonSpriteVarShiny.sprite = pokemonScriptable.pokemonSpriteShinyVariation;

        UpdateEncountersAndCatches();

        maxFormView = 1;
        if (pokemonScriptable.canBeAltForm) maxFormView = 3;
        
        var sprites = new[] { pokemonSprite, pokemonSpriteShiny, pokemonSpriteVar, pokemonSpriteVarShiny };
        DisableAllSpriteExcept(pokemonSprite, sprites);
    }

    public void UpdateEncountersAndCatches()
    {
        foreach (var pkmnData in PokemonManager.instance.localPlayer.pokemonDatas)
        {
            if (pkmnData.pokemonScriptable == pokemonScriptable)
            {
                pokemonName.text = pkmnData.encounters > 0 || pkmnData.encountersShiny > 0 ? $"{pokemonScriptable.pokemonName} #{pokemonScriptable.pokemonID:000}" : "???";
                
                pokemonSprite.color = pkmnData.encounters > 0 ? Color.white : Color.black;
                pokemonSpriteShiny.color = pkmnData.encountersShiny > 0 ? Color.white : Color.black;
                
                pokemonSpriteVar.color = pkmnData.encountersVar > 0 ? Color.white : Color.black;
                pokemonSpriteVarShiny.color = pkmnData.encountersVarShiny > 0 ? Color.white : Color.black;

                if (!pokemonScriptable.canBeAltForm)
                {
                    pokemonSpriteVar.DOFade(0, 0);
                    pokemonSpriteVarShiny.DOFade(0, 0);
                }
                
                pokemonEncounters.text = pkmnData.encounters.ToString();
                pokemonCatch.text = pkmnData.catches.ToString();
            }
        }
    }

    public void SwitchForm()
    {
        if (currentFormView + 1 <= maxFormView) currentFormView++;
        else currentFormView = 0;

        var sprites = new[] { pokemonSprite, pokemonSpriteShiny, pokemonSpriteVar, pokemonSpriteVarShiny };
        switch (currentFormView)
        {
            case 0:
                DisableAllSpriteExcept(pokemonSprite, sprites);
                pokemonShinyStar.enabled = false;
                break;
            case 1:
                DisableAllSpriteExcept(pokemonSpriteShiny, sprites);
                pokemonShinyStar.enabled = true;
                break;
            case 2:
                DisableAllSpriteExcept(pokemonSpriteVar, sprites);
                pokemonShinyStar.enabled = false;
                break;
            case 3:
                DisableAllSpriteExcept(pokemonSpriteVarShiny, sprites);
                pokemonShinyStar.enabled = true;
                break;
        }
    }

    private void DisableAllSpriteExcept(Image s, Image[] sprites)
    {
        foreach (var spr in sprites)
        {
            spr.SetNativeSize();
            spr.SetNativeSize();
        }

        foreach (var spr in sprites)
        {
            spr.gameObject.SetActive(false);
        }
        
        s.gameObject.SetActive(true);
        
        s.transform.localScale = new Vector3(-1,1,1) * 1.5f;
        s.transform.DOScaleX(1.5f, 0.35f);
        s.transform.DOLocalJump(s.transform.localPosition, 50f, 1, 0.35f);
    }
}
