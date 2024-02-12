using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkFollow : NetworkBehaviour
{
    private Transform target;

    public void Init(Transform t) => target = t;

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position;
    }
}
