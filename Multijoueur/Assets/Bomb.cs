using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class Bomb : NetworkBehaviour
{
    [SerializeField] private float timeBeforeExplose;
    [SerializeField] private GameObject explodeVFX;
    
    private NetworkAnimator _animator;
    private float currentSpeed = 1f;

    public override void OnNetworkSpawn()
    {
        _animator = GetComponent<NetworkAnimator>();
        StartCoroutine(nameof(Explode));
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
        
        GetComponent<NetworkObject>().Despawn();
    }
    
    [ServerRpc]
    void SpawnExplodeVFX_ServerRpc(Vector3 pos)
    {
        SpawnExplodeVFX_ClientRpc(pos);
    }
    
    [ClientRpc]
    void SpawnExplodeVFX_ClientRpc(Vector3 pos)
    {
        GameObject eVFX = Instantiate(explodeVFX, pos, Quaternion.identity);
        eVFX.GetComponent<NetworkObject>().Spawn(true); // Spawn on the network
    }
}
