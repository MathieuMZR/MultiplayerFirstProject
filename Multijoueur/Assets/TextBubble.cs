using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextBubble : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private AudioSource textSource, popSource;

    public void GenerateBubble(string text)
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.35f);
        
        TextChanger.Instance.ModifyTextByPokémonType(ref tmp, text);

        StartCoroutine(GenerateTextSounds(text, 1f / (TextChanger.Instance.RemoveRichText(text).Length / 2f)));
    }

    private IEnumerator GenerateTextSounds(string text, float delay)
    {
        popSource.Play();
        for (int i = 0; i < TextChanger.Instance.RemoveRichText(text).Length; i += 2)
        {
            textSource.pitch = Random.Range(0.95f, 1.05f);
            textSource.Play();
            yield return new WaitForSeconds(delay);
        }
    }
}
