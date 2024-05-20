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
    
    public static void DrawBoxCollider(Color gizmoColor, Transform t, BoxCollider boxCollider, float alphaForInsides = 0.3f)
    {
        //Save the color in a temporary variable to not overwrite changes in the inspector (if the sent-in color is a serialized variable).
        var color = gizmoColor;
 
        //Change the gizmo matrix to the relative space of the boxCollider.
        //This makes offsets with rotation work
        Gizmos.matrix = Matrix4x4.TRS(t.TransformPoint(boxCollider.center), t.rotation, t.lossyScale);
 
        //Draws the edges of the BoxCollider
        //Center is Vector3.zero, since we've transformed the calculation space in the previous step.
        Gizmos.color = color;
        Gizmos.DrawWireCube(Vector3.zero, boxCollider.size);
 
        //Draws the sides/insides of the BoxCollider, with a tint to the original color.
        color.a *= alphaForInsides;
        Gizmos.color = color;
        Gizmos.DrawCube(Vector3.zero, boxCollider.size);
    }
    
    public static void DrawBoxCollider(Color gizmoColor, Transform t, Vector3 halfExtends, float alphaForInsides = 0.3f)
    {
        //Save the color in a temporary variable to not overwrite changes in the inspector (if the sent-in color is a serialized variable).
        var color = gizmoColor;
 
        //Change the gizmo matrix to the relative space of the boxCollider.
        //This makes offsets with rotation work
        Gizmos.matrix = Matrix4x4.TRS(t.position, t.rotation, t.lossyScale);
 
        //Draws the edges of the BoxCollider
        //Center is Vector3.zero, since we've transformed the calculation space in the previous step.
        Gizmos.color = color;
        Gizmos.DrawWireCube(Vector3.zero, halfExtends);
 
        //Draws the sides/insides of the BoxCollider, with a tint to the original color.
        color.a *= alphaForInsides;
        Gizmos.color = color;
        Gizmos.DrawCube(Vector3.zero, halfExtends);
    }
    
    public static void DecreaseTimerIfPositive(this ref float f, float multiplier = 1f)
    {
        if (f > 0) f -= Time.deltaTime * multiplier;
        if (f < 0) f = 0;
    }
    
    public static void IncreaseTimerIfPositive(this ref float f, float multiplier = 1f)
    {
        if (f < 1) f += Time.deltaTime * multiplier;
        if (f > 1) f = 1;
    }
}
