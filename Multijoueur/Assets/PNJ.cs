using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class PNJ : NetworkBehaviour
{
    [SerializeField] private string text;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Transform[] talkPoints;
    [SerializeField] private bool invertScale;
    
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private Canvas input;

    private NetworkVariable<bool> dialogInitiated = 
        new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.F) && !dialogInitiated.Value)
        {
            if (other.CompareTag("Player"))
            {
                SpriteRotation();
                
                if (!other.GetComponent<PlayerController>().IsOwner) return;
                if (!IsSpawned) return; 
                    
                DialogStarted_rpc(true);
                
                StartCoroutine(TextRoutine(other));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!other.GetComponent<PlayerController>().IsOwner) return;

            input.enabled = true;
            
            input.transform.DOScale(Vector3.zero, 0f);
            input.transform.DOScale(Vector3.one / 1000f, 0.25f);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!other.GetComponent<PlayerController>().IsOwner) return;
            
            input.transform.DOScale(Vector3.zero, 0.25f).OnComplete(()=> input.enabled = false);
        }
    }

    private void SpriteRotation()
    {
        List<PlayerController> players = new List<PlayerController>();
        players = FindObjectsOfType<PlayerController>().ToList();

        foreach (PlayerController p in players.OrderBy(c =>
                     Vector2.Distance(transform.position, c.transform.position)))
        {
            if (p != players[0]) continue;
            var axis = p.transform.position.x > transform.position.x ? -1 : 1;
            sprite.transform.DOScaleX(axis * (invertScale ? -1 : 1), 0.35f);
        }
    }

    IEnumerator TextRoutine(Collider other)
    {
        var player = other.GetComponent<PlayerController>();

        cam.Priority = 1;
        player.vc.Priority = 0;
        
        input.transform.DOScale(Vector3.zero, 0.25f).OnComplete(()=> input.enabled = false);
        
        player.allowInputs = false;
        player.walkHardCoded = true;

        var index = player.transform.position.x > transform.position.x ? 0 : 1;
        
        player.direction = index == 0 ? -Vector3.right : Vector3.right;
        player.lastDirection = index == 0 ? -Vector3.right : Vector3.right;
        cam.transform.DOLocalMoveX(talkPoints[index].localPosition.x / 2f, 0.75f);
        
        player.transform.DOMove(talkPoints[index].position, 0.75f).SetEase(Ease.OutSine)
            .OnComplete(()=>
            {
                player.walkHardCoded = false;
            });

        yield return new WaitForSeconds(0.75f);
            
        TextChanger.Instance.GenerateTextBubble(transform, text);

        yield return new WaitForSeconds(text.Length / 15f);
        
        cam.Priority = 0;
        player.vc.Priority = 1;
        
        player.allowInputs = true;

        yield return new WaitForSeconds(2f);
        
        DialogStarted_rpc(false);
    }

    [Rpc(SendTo.Server)]
    void DialogStarted_rpc(bool value)
    {
        dialogInitiated.Value = value;
    }
}
