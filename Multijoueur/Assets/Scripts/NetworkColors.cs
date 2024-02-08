using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkColors : MonoBehaviour
{
    public static NetworkColors Singleton;
    public Texture2D[] playerTextures;
    private void Awake() => Singleton = this;
}
