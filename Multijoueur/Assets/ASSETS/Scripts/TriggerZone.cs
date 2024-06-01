using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
public class TriggerZone : MonoBehaviour
{
    [SerializeField] private string zoneName;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController p = other.GetComponent<PlayerController>();
            Text t = p.zoneAnimator.GetComponentInChildren<Text>();

            if (t.text != zoneName)
            {
                p.zoneAnimator.SetTrigger("EnterNewZone");
                t.text = zoneName;
            }
        }
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Helpers.DrawBoxCollider(Color.cyan, transform, GetComponent<BoxCollider>(), 0.05f);
        Handles.Label(transform.position, zoneName, new GUIStyle(){fontSize = 30});
    }
    #endif
}
