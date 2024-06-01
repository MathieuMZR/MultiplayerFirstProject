using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomCodeCanvas : MonoBehaviour
{
    [SerializeField] private Button quitGameButton;
    [SerializeField] private GameObject infosGame;
    
    // Start is called before the first frame update
    void Start()
    {
        quitGameButton.onClick.AddListener(Application.Quit);
        PokemonManager.instance.OnLocalPlayerJoined += () =>
        {
            if (PokemonManager.instance.localPlayer.IsClient && !PokemonManager.instance.localPlayer.IsHost) infosGame.SetActive(false);
            GetComponent<Canvas>().enabled = true;
        };
    }
}
