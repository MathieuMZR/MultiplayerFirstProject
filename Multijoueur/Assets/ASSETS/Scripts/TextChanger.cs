using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TextChanger : GenericSingletonClass<TextChanger>
{
    public TextData[] textDatas;
    public TextDataFromType[] typeDatas;
    public TextMeshProUGUI bbb;
    
    [SerializeField] private float amplitude = 1f;
    [SerializeField] private float frequency = 1f;
    [SerializeField] private float speed = 1f;
    
    [SerializeField] private AnimationCurve curve;
    
    [SerializeField] private TextBubble textBubble;

    public void GenerateTextBubble(Transform t, string s)
    {
        var obj = Instantiate(textBubble, t);
        obj.GenerateBubble(s);
    }

    private string EnhancedText(string textToChange)
    {
        string[] sArray = textToChange.Split();
        string value = "";
        
        foreach (string s in sArray)
        {
            foreach (TextData t in textDatas)
            {
                if (s == t.word)
                {
                    value += $"<color=#{ColorUtility.ToHtmlStringRGB(t.color)}> {s}</color>";
                    break;
                }
                else
                {
                    value += $" {s}";
                    break;
                }
            }
        }

        return value;
    }
    private string EnhancedTextFromPokemonType(string textToChange)
    {
        string[] sArray = textToChange.Split();
        string value = "";
        string pkmnNameFound = "";
        
        foreach (string s in sArray)
        {
            foreach (Pokemon_SO pkmn in PokemonManager.instance.allPokemon)
            {
                if (s == pkmn.pokemonName)
                {
                    foreach (TextDataFromType t in typeDatas)
                    {
                        if (t.type == pkmn.pokemonType)
                        {
                            value += $"<color=#{ColorUtility.ToHtmlStringRGB(t.color)}> {s}</color>";
                            pkmnNameFound = pkmn.pokemonName;
                            break;
                        }
                    }
                    break;
                }
            }
            if(s != pkmnNameFound)
                value += $" {s}";
        }

        return value;
    }
    
    public void ModifyText(ref TextMeshProUGUI t, string textToChange)
    {
        string s = textToChange;
        t.text = "";
        t.DOText(EnhancedText(s), 1f).SetEase(Ease.Linear);
    }
    
    public void ModifyTextByPokémonType(ref TextMeshProUGUI t, string textToChange)
    {
        string s = textToChange;
        Debug.Log(t);
        t.text = "";
        t.DOText(EnhancedTextFromPokemonType(s), 1f).SetEase(Ease.Linear);
    }
    
    public void WiggleAllTexts()
    {
        foreach (TextMeshProUGUI t in FindObjectsOfType<TextMeshProUGUI>())
        {
            //if (!t.transform.parent.gameObject.activeInHierarchy) continue;
            
            var vertices = new Vector3[]{};
            var textInfo = t.textInfo;
            
            t.ForceMeshUpdate();

            string pokemonName = "";
            int beginIndex = 0;
            foreach (Pokemon_SO pkmn in PokemonManager.instance.allPokemon)
            {
                if (t.text.Contains(pkmn.pokemonName))
                {
                    pokemonName = pkmn.pokemonName;
                    beginIndex = RemoveRichText(t.text).IndexOf(pokemonName, StringComparison.Ordinal);
                    break;
                }
            }

            for (int i = 0; i < t.text.Length; i++)
            {
                if (i >= beginIndex + pokemonName.Length ||
                    i < beginIndex) continue;

                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                int vertexIndex = charInfo.vertexIndex;

                vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

                var y = Mathf.Round(Mathf.Sin(Time.time * speed + i * frequency));
                Vector3 offset = new Vector3(0, y * amplitude);

                vertices[vertexIndex + 0] += offset;
                vertices[vertexIndex + 1] += offset;
                vertices[vertexIndex + 2] += offset;
                vertices[vertexIndex + 3] += offset;
            }

            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                t.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }
        }
    }
    
    private void Update()
    {
        WiggleAllTexts();
    }

    public string RemoveRichText(string str)
    {
        var splitWithoutLBracket = str.Split("<");
        List<string> final = new List<string>();
        foreach (var s in splitWithoutLBracket)
        {
            var splitWithoutRBracket = s.Split(">");
            final.Add(splitWithoutRBracket[^1]);
        }

        var finalStr = string.Join("", final);

        return finalStr;
    }
}

[Serializable]
public class TextData
{
    public string word;
    public Color color;
}

[Serializable]
public class TextDataFromType
{
    public Pokemon_SO.PokemonType type;
    public Color color;
}
