using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : GenericSingletonClass<HUD>
{
    public Canvas canvas;
    
    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<Canvas>();
    }
}
