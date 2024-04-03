using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EmoteWheel : NetworkBehaviour
{
    [SerializeField] private List<Vector3> emotePositions;
    [SerializeField] private Color[] wheelColors;
    [SerializeField] private Image wheelBackground;

    private Vector3 _lastCursorPosition;
    private Vector3 nearestVector3;
    private int currentIndexPosition;
    
    // Update is called once per frame
    void Update()
    {
        _lastCursorPosition = Input.mousePosition;
        CalculateNearestVector3InWheel();

        wheelBackground.color =
            Color.Lerp(wheelBackground.color, wheelColors[currentIndexPosition], Time.deltaTime * 10f);
    }

    private void OnDisable()
    {
        if (GetComponentInParent<N_PlayerEmotes>().isEmoting) return;
        
        Debug.Log("EmoteRPC");
        GetComponentInParent<N_PlayerController>().Emote_Rpc(currentIndexPosition);
    }

    void CalculateNearestVector3InWheel()
    {
        // Convert screen coordinates to local coordinates relative to the UI element
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            GetComponent<RectTransform>(),
            _lastCursorPosition,
            null,
            out var cursorLocalPosition);
        
        nearestVector3 = FindNearestVector(cursorLocalPosition, emotePositions.ToArray());
        currentIndexPosition = emotePositions.IndexOf(nearestVector3);
    }
    
    private Vector3 FindNearestVector(Vector3 target, Vector3[] vectorList)
    {
        if (vectorList == null || vectorList.Length == 0)
        {
            Debug.LogWarning("Vector list is empty.");
            return Vector3.zero;
        }

        Vector3 nearestVector = vectorList[0];
        float shortestDistance = Vector3.Distance(target, vectorList[0]);

        for (int i = 1; i < vectorList.Length; i++)
        {
            float distance = Vector3.Distance(target, vectorList[i]);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestVector = vectorList[i];
            }
        }
        
        return nearestVector;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if (emotePositions.Count == 0) return;
        foreach (var v in emotePositions)
        {
            Gizmos.DrawSphere(transform.position + v, 10f);
        }
    }
}
