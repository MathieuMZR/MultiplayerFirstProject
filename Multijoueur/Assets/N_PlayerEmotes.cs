using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class N_PlayerEmotes : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer face;
    [SerializeField] private Sprite[] faceSprites;

    public bool isEmoting;

    public enum PlayerFaceType
    {
        Fine,
        Determined,
        Inquisitive,
        Worried,
        InLove,
        Dead,
        Kind,
        Angry,
        Sad,
        Chocked
    }
    
    public void SetNewFace(int index)
    {
        face.sprite = faceSprites[index];
    }

    public IEnumerator StartNewEmote(int index)
    {
        isEmoting = true;
        
        SetNewFace(index);
        yield return new WaitForSeconds(1.5f); //Emote duration
        SetNewFace(0);

        isEmoting = false;
    }
}
