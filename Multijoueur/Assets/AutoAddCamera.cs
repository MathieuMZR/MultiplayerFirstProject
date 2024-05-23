using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AutoAddCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PokemonManager.instance.OnLocalPlayerJoined += () =>
        {
            if (!PokemonManager.instance.localPlayer.IsOwner) return;
            GetComponent<Canvas>().worldCamera = PokemonManager.instance.localPlayer.listener;
            GetComponent<Canvas>().planeDistance = 1f;
        };
    }
}
