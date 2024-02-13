using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(NetworkObject))]

public class Prop : NetworkBehaviour, INetworkSerializable
{
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        
    }
}
