using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitlScreenButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] float sizeMultiplier;
    [SerializeField] AnimationCurve curve;
    [SerializeField] float duration;

    private Vector3 baseScale;

    private void Start()
    {
        baseScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(baseScale * sizeMultiplier, duration).SetEase(curve);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(baseScale, duration).SetEase(curve);
    }
}
