using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class DropDownCode : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private RectTransform t;
    [SerializeField] private GameObject commitButton;
    [SerializeField] private Vector2[] animVectors;
    [SerializeField] private float duration;
    [SerializeField] private AnimationCurve curve;

    private Vector2 baseSizeDelta;

    private bool flipFlop;

    private void Start()
    {
        baseSizeDelta = t.sizeDelta;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!flipFlop)
        {
            t.DOSizeDelta(new Vector2(baseSizeDelta.x, animVectors[0].x), duration)
                .SetEase(curve);
            t.DOAnchorPosY(animVectors[0].y, duration).SetEase(curve).OnComplete(() =>
            {
                commitButton.SetActive(true);
                commitButton.transform.DOScale(Vector3.one, duration);
            });
        }
        else
        {
            t.DOSizeDelta(new Vector2(baseSizeDelta.x, baseSizeDelta.y), duration)
                .SetEase(curve);
            t.DOAnchorPosY(animVectors[1].y, duration).SetEase(curve);
            commitButton.transform.DOScale(Vector3.zero, duration / 2f).OnComplete(() =>
            {
                commitButton.SetActive(false);
            });
        }

        flipFlop = !flipFlop;
    }
}
