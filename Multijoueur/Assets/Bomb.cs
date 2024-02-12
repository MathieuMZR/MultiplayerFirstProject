using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEditor;
using UnityEngine;

public class Bomb : NetworkBehaviour
{
    public NetworkVariable<int> ownerID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);
    
    [SerializeField] private float timeBeforeExplose;
    [SerializeField] private GameObject explodeVFX;

    private int ID;
    
    private NetworkAnimator _animator;
    private float currentSpeed = 1f;

    public override void OnNetworkSpawn()
    {
        _animator = GetComponent<NetworkAnimator>();
        StartCoroutine(nameof(Explode));

        StartCoroutine(nameof(LateNetworkSpawn));
    }

    IEnumerator LateNetworkSpawn()
    {
        yield return new WaitForSeconds(0.01f);
        ColorBomb(ownerID.Value);
    }

    public void ColorBomb(int i)
    {
        foreach (MeshRenderer r in GetComponentsInChildren<MeshRenderer>())
        {
            r.material.SetColor("_Color", NetworkColors.Singleton.playerColors[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentSpeed += Time.deltaTime / (timeBeforeExplose / 2f);
        transform.localScale += Vector3.one * Time.deltaTime / (timeBeforeExplose * 2f);
        
        _animator.Animator.speed = currentSpeed;
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(timeBeforeExplose);
        SpawnExplodeVFX_ServerRpc(transform.position);
        
        if (IsHost)
        {
            SpawnExplodeVFX(transform.position);
            GetComponent<NetworkObject>().Despawn();
        }
        else
        {
            SpawnExplodeVFX_ServerRpc(transform.position);
        }
    }
    
    [ServerRpc]
    void SpawnExplodeVFX_ServerRpc(Vector3 pos)
    {
        SpawnExplodeVFX(pos); // Spawn on the network
    }
    
    void SpawnExplodeVFX(Vector3 pos)
    {
        GameObject eVFX = Instantiate(explodeVFX, pos, Quaternion.identity);
        eVFX.GetComponent<NetworkObject>().Spawn(true); 
    }

    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        Handles.Label(transform.position, OwnerClientId.ToString());
        #endif
    }
}
