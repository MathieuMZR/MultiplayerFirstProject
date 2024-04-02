using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class N_PlayerNameTag : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI nameTagText;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if(!nameTagText) Helpers.ErrorReferenceInspectorFromGo(gameObject);
        nameTagText.text = "ClientID " + OwnerClientId;
    }
}
