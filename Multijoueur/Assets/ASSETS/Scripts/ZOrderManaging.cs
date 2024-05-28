using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZOrderManaging : MonoBehaviour
{
    [SerializeField] private int offset;

    private SpriteRenderer srObj;

    private void Start()
    {
        if (TryGetComponent(out SpriteRenderer sr))
        {
            srObj = sr;
        }
    }
    
    void Update()
    {
        var pos = (int)transform.root.position.z + offset;

        if (srObj is null || srObj.sortingOrder == pos) return;
        srObj.sortingOrder = -pos;
    }
}
