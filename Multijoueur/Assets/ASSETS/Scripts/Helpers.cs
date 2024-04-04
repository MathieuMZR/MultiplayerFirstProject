using System;
using UnityEngine;

public static class Helpers
{
    public static void ErrorReferenceFromGo(MonoBehaviour script)
    {
        throw new Exception($"Missing reference in <color=yellow>{script.name}</color> script.");
    }
    
    public static void ErrorReferenceInspectorFromGo(GameObject obj)
    {
        throw new Exception($"Missing reference in <color=yellow>{obj.name}</color>'s inspector.");
    }
}
