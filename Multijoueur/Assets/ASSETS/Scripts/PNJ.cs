using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PNJ : NetworkBehaviour
{
    [SerializeField] private string text;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Transform[] talkPoints;

    [SerializeField] private Canvas canvasInput;
    [SerializeField] private Image inputSprite;

    private NetworkVariable<bool> dialogInitiated = 
        new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private bool isInTrigger;
    private PlayerController inTriggerPlayer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !dialogInitiated.Value && isInTrigger && inTriggerPlayer != null)
        {
            if (!inTriggerPlayer.IsOwner) return;
            
            SpriteRotation();

            if(inTriggerPlayer.IsServer) dialogInitiated.Value = true;
            
            StartCoroutine(TextRoutine(inTriggerPlayer));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!other.GetComponent<PlayerController>().IsOwner) return;

            isInTrigger = true;
            inTriggerPlayer = other.GetComponent<PlayerController>();
            
            inputSprite.enabled = true;

            inputSprite.transform.DOScale(Vector3.zero, 0f);
            inputSprite.transform.DOScale(Vector3.one, 0.25f);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!other.GetComponent<PlayerController>().IsOwner) return;

            inTriggerPlayer = null;

            isInTrigger = false;
            inputSprite.transform.DOScale(Vector3.zero, 0.25f).OnComplete(()=> inputSprite.enabled = false);
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
            var axis = (transform.position - p.transform.position).normalized;
            sprite.transform.DOScaleX((axis.x > 0 ? 1 : -1) * (sprite.flipX ? -1 : 1), 0.35f);
        }
    }

    IEnumerator TextRoutine(PlayerController other)
    {
        var player = other;
        
        inputSprite.transform.DOScale(Vector3.zero, 0.25f).OnComplete(()=> inputSprite.enabled = false);
        
        player.allowInputs = false;
        player.walkHardCoded = true;

        var index = player.transform.position.x > transform.position.x ? 0 : 1;
        
        player.direction = index == 0 ? -Vector3.right : Vector3.right;
        player.lastDirection = index == 0 ? -Vector3.right : Vector3.right;
        //player.listener.transform.DOLocalMoveX(talkPoints[index].localPosition.x / 2f, 0.75f);
        
        player.transform.DOMove(talkPoints[index].position, 0.75f).SetEase(Ease.OutSine)
            .OnComplete(()=>
            {
                player.walkHardCoded = false;
            });

        yield return new WaitForSeconds(0.75f);
            
        TextChanger.Instance.GenerateTextBubble(HUD.Instance.transform, text);

        yield return new WaitForSeconds(text.Length / 15f);
        
        player.allowInputs = true;

        yield return new WaitForSeconds(2f);
        
        //input.transform.DOScale(Vector3.one / 1000f, 0.25f);
        
        if(player.IsServer) dialogInitiated.Value = false;
    }
}
