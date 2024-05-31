using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRain : MonoBehaviour
{
    [SerializeField] private GameObject rain;

    public void EnableRain(bool enable) => rain.SetActive(enable);
}
