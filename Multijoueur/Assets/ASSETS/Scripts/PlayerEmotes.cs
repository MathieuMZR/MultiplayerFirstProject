using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEmotes : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer face;
    [SerializeField] private Image bubbleExpression;
    [SerializeField] private GameObject bubbleExpressionParent;
    [SerializeField] private List<EmoteComposite> emoteComposites = new List<EmoteComposite>();

    public bool isEmoting;
    
    private void SetNewFace(int index)
    {
        face.sprite = emoteComposites[index].faceSprite;
        bubbleExpression.sprite = emoteComposites[index].bubbleIconSprite;
    }

    public IEnumerator StartNewEmote(int index)
    {
        var transform1 = bubbleExpressionParent.transform;
        isEmoting = true;
        
        SetNewFace(index);
        transform1.gameObject.SetActive(true);
        transform1.localScale = Vector3.zero;
        transform1.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);
        
        yield return new WaitForSeconds(1.5f); //Emote duration
        
        transform1.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutSine).OnComplete(()=>
        transform1.gameObject.SetActive(false));

        yield return new WaitForSeconds(0.5f);

        isEmoting = false;
        SetNewFace(0);
    }
}

[Serializable]
public class EmoteComposite
{
    public Sprite faceSprite, bubbleIconSprite;
}
