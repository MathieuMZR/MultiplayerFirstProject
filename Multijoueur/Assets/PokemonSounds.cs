using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonSounds : MonoBehaviour
{
    [SerializeField] private AudioSource shinyAudioSource;
    [SerializeField] private AudioSource popAudioSource;
    [SerializeField] private AudioSource popAudioSource2;
    
    [SerializeField] private AudioClip[] popSFXs;

    public void AppearSound(bool isShiny)
    {
        if (isShiny) shinyAudioSource.Play();
        popAudioSource.PlayOneShot(popSFXs[Random.Range(0, popSFXs.Length)]);

        popAudioSource2.pitch = Random.Range(0.8f, 1.2f);
        popAudioSource2.PlayDelayed(0.2f);
    }
}
