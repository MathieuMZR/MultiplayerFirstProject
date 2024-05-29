using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class AutoAddCamera : MonoBehaviour
{
    [SerializeField] private bool addOnStart;
    
    // Start is called before the first frame update
    void Start()
    {
        PokemonManager.instance.OnLocalPlayerJoined += () =>
        {
            if (!PokemonManager.instance.localPlayer.IsOwner) return;
            GetComponent<Canvas>().worldCamera = PokemonManager.instance.localPlayer.listener;
            GetComponent<Canvas>().planeDistance = 1.2f;
        };

        if (addOnStart)
        {
            if (!PokemonManager.instance.localPlayer.IsOwner) return;
            GetComponent<Canvas>().worldCamera = PokemonManager.instance.localPlayer.listener;
            GetComponent<Canvas>().planeDistance = 1.2f;
        }
    }
}
