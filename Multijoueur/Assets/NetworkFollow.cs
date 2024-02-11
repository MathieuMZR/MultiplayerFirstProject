using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkFollow : NetworkBehaviour
{
    [SerializeField] private float followSpeed = 50;
    private Transform target;

    public void Init(Transform t) => target = t;

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
    }
}
