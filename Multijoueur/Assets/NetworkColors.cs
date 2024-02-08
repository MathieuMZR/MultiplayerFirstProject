using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkColors : MonoBehaviour
{
    public static NetworkColors Singleton;
    public Color[] playerColors;
    private void Awake() => Singleton = this;
}
