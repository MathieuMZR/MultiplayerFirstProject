using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    public static Vector3 IsoConvertVector(Vector3 vector, float isoAngle = 45f)
    {
        Quaternion rotation = Quaternion.Euler(0,isoAngle,0);
        Matrix4x4 isoMatrix = Matrix4x4.Rotate(rotation);
        Vector3 result = isoMatrix.MultiplyPoint3x4(vector);
        return result;
    }
}
