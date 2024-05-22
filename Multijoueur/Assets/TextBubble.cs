using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextBubble : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private AudioSource textSource, popSource, unpopSource;
    [SerializeField] private AnimationCurve curveAnim;

    public void GenerateBubble(string text)
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.35f);

        var rect = GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, -150);
        rect.DOAnchorPosY(50, 0.75f).SetEase(curveAnim);
        
        TextChanger.Instance.ModifyTextByPokémonType(ref tmp, text);

        StartCoroutine(GenerateTextSounds(text, 1f / (TextChanger.Instance.RemoveRichText(text).Length / 2f)));
        
        KillTextBubble(TextChanger.Instance.RemoveRichText(text).Length / 10f);
    }

    private void KillTextBubble(float delay)
    {
        unpopSource.PlayDelayed(delay);
        
        var rect = GetComponent<RectTransform>();
        rect.DOAnchorPosY(-150, 0.75f).SetDelay(delay).SetEase(curveAnim).OnComplete(() =>
        {
            Destroy(gameObject);
        });
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
