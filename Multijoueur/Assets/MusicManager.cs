using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MusicManager : GenericSingletonClass<MusicManager>
{
    [SerializeField] private AudioSource grassLandMusic;
    [SerializeField] private AudioSource battleMusic;

    private List<float> baseVolumes = new List<float>();

    private void Start()
    {
        baseVolumes.Add(grassLandMusic.volume);
        baseVolumes.Add(battleMusic.volume);
        
        GrassLandMusic();
    }

    public void GrassLandMusic()
    {
        grassLandMusic.Play();
        battleMusic.Stop();
        
        grassLandMusic.DOFade(baseVolumes[0], 0.3f);
        battleMusic.DOFade(0, 0.3f);
    }
    
    public void BattleMusic()
    {
        battleMusic.Play();
        grassLandMusic.Stop();
        
        battleMusic.DOFade(baseVolumes[1], 0.3f);
        grassLandMusic.DOFade(0, 0.3f);
    }
}
