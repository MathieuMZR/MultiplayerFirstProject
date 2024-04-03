using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class N_Pkmn : MonoBehaviour
{
    public Pkmn_SO pkmnScriptable;

    public void InitiateScriptable(Pkmn_SO pkmn) => pkmnScriptable = pkmn;

    // Update is called once per frame
    void Update()
    {
        
    }
}
