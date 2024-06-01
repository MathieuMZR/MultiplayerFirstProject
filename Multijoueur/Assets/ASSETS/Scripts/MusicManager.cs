using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MusicManager : GenericSingletonClass<MusicManager>
{
    [SerializeField] private AudioSource grassLandMusic;
    [SerializeField] private AudioSource battleMusic;
    [SerializeField] private AudioSource titleMusic;

    [SerializeField] private CrossFadeData[] crossFadeDatas;

    [SerializeField] private List<float> baseVolumes = new List<float>();

    private void Start()
    {
        TitleMusic();

        PokemonManager.instance.OnLocalPlayerJoined += ()=> GrassLandMusic(true);
    }

    public void GrassLandMusic(bool restart)
    {
        if(restart) crossFadeDatas[0].source1.Play();

        crossFadeDatas[0].source1.DOFade(baseVolumes[0], crossFadeDatas[0].fadeSpeed);
        battleMusic.DOFade(0, crossFadeDatas[0].fadeSpeed);
        titleMusic.DOFade(0, crossFadeDatas[0].fadeSpeed);
    }
    
    public void BattleMusic()
    {
        crossFadeDatas[1].source1.Play();
        
        crossFadeDatas[1].source1.DOFade(baseVolumes[1], crossFadeDatas[1].fadeSpeed);
        grassLandMusic.DOFade(0, crossFadeDatas[1].fadeSpeed);
        titleMusic.DOFade(0, crossFadeDatas[1].fadeSpeed);
    }
    
    public void TitleMusic()
    {
        crossFadeDatas[2].source1.DOFade(baseVolumes[2], crossFadeDatas[2].fadeSpeed);
        battleMusic.DOFade(0, crossFadeDatas[2].fadeSpeed);
        grassLandMusic.DOFade(0, crossFadeDatas[2].fadeSpeed);
    }
    
    public void FadeAllMusics()
    {
        titleMusic.DOFade(0, 1);
        battleMusic.DOFade(0, 1);
        grassLandMusic.DOFade(0, 1);
    }
}

[Serializable]
public class CrossFadeData
{
    public AudioSource source1;
    public float fadeSpeed = 0.25f;
}
