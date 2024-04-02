using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class N_HostJoinFunctions : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!hostButton || !joinButton) Helpers.ErrorReferenceInspectorFromGo(gameObject);
            
        hostButton.onClick.AddListener(()=>NetworkManager.Singleton.StartHost());
        joinButton.onClick.AddListener(()=>NetworkManager.Singleton.StartClient());
    }
}
