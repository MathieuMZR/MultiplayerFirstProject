using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.EventSystems;

public class BattleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool isCatchButton;
    [SerializeField] private TextMeshProUGUI hint;
    [SerializeField] private float offsetHintY;
    
    [SerializeField] private float scale = 1.1f;
    [SerializeField] private float animSpeed = 1f;
    
    private bool isInButton;

    private Vector3 baseScale;
    private Vector2 basePosHint;
    private float _timerOffsetY;

    private RectTransform rt;
    private RectTransform rtHint;
    
    void Start()
    {
        rt = GetComponent<RectTransform>();
        baseScale = rt.transform.localScale;

        rtHint = hint.GetComponent<RectTransform>();
        basePosHint = rtHint.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        rt.transform.localScale = Vector3.Lerp(baseScale, baseScale * scale, _timerOffsetY);
        _timerOffsetY = Mathf.Clamp(_timerOffsetY + Time.deltaTime * animSpeed * (!isInButton ? -1 : 1), 0, 1);
        
        rtHint.anchoredPosition = Vector3.Lerp(basePosHint, 
            basePosHint + new Vector2(0, offsetHintY), _timerOffsetY);
        hint.alpha = Mathf.Lerp(0, 1, _timerOffsetY);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isInButton = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isInButton = false;
    }
}
