using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WeatherManager : GenericSingletonClass<WeatherManager>
{
    [SerializeField] private DecalProjector droplets;
    [SerializeField] private Vector2 minMaxTime;
    [SerializeField] private float rainChance;

    public bool isRaining;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WeatherRoutine());
    }

    IEnumerator WeatherRoutine()
    {
        yield return new WaitForSeconds(Random.Range(minMaxTime.x, minMaxTime.y));

        if (Random.value < rainChance)
        {
            Rain_rpc();
            isRaining = true;
        }
        else
        {
            StopRain_rpc();
            isRaining = false;
        }
        
        StartCoroutine(WeatherRoutine());
    }

    [Rpc(SendTo.Everyone)]
    private void Rain_rpc()
    {
        foreach (PlayerRain p in FindObjectsOfType<PlayerRain>())
        {
            p.EnableRain(true);
        }

        DOTween.To(() => droplets.fadeFactor, x => droplets.fadeFactor = x, 1f, 6);
    }
    
    [Rpc(SendTo.Everyone)]
    private void StopRain_rpc()
    {
        foreach (PlayerRain p in FindObjectsOfType<PlayerRain>())
        {
            p.EnableRain(false);
        }
        
        DOTween.To(() => droplets.fadeFactor, x => droplets.fadeFactor = x, 0f, 6);
    }
}
