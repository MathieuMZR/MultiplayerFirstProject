using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerUIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button clientButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button serverButton;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TextMeshProUGUI partyJoinCode;
    [SerializeField] private TMP_InputField joinCodeInput;
    
    private void Start()
    {
        //clientButton.onClick.AddListener(()=> JoinRoom(joinCodeInput.text));
        quitButton.onClick.AddListener(Application.Quit);
        submitButton.onClick.AddListener(()=> JoinRoom(joinCodeInput.text));
        joinCodeInput.onSubmit.AddListener(JoinRoom);
        
        hostButton.onClick.AddListener(async () =>
        {
            if (RelayManager.Instance.IsRelayEnabled)
            {
                MusicManager.Instance.FadeAllMusics();
                
                RelayHostData hostData = await RelayManager.Instance.SetupRelay();
                partyJoinCode.text = hostData.JoinCode;
            }

            NetworkManager.Singleton.StartHost();
            gameObject.SetActive(false);
        });
        //serverButton.onClick.AddListener(() => NetworkManager.Singleton.StartServer());
    }

    async void JoinRoom(string t)
    {
        if (string.IsNullOrEmpty(t)) return;
            
        if (RelayManager.Instance.IsRelayEnabled && !string.IsNullOrEmpty(t))
        {
            MusicManager.Instance.FadeAllMusics();
            await RelayManager.Instance.JoinRelay(t);
        }

        NetworkManager.Singleton.StartClient();
        gameObject.SetActive(false);
    }
}