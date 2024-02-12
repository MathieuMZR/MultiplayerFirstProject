using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(NetworkObject))]
public class Prop : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (PlayerList.Singleton.IsAPlayer(other))
        {
            //Get the player network component from the collided actor.
            var p = other.GetComponent<PlayerNetwork>();
            AddPropToPlayerPossibleInteractions(p);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (PlayerList.Singleton.IsAPlayer(other))
        {
            //Get the player network component from the collided actor.
            var p = other.GetComponent<PlayerNetwork>();
            RemovePropToPlayerPossibleInteractions(p);
        }
    }
    
    
    /// <summary>
    /// Add the prop to the player possible interactions list.
    /// </summary>
    /// <param name="p">The Player concerned by the adding.</param>
    private void AddPropToPlayerPossibleInteractions(PlayerNetwork p)
    {
        //Check if the prop isn't already in the list.
        if (!p.allAvailableProps.Contains(this))
        {
            //Add the prop to the interactable props.
            p.allAvailableProps.Add(this);
            
            //Get the nearest prop and actualize it if necessary
            p.GetNearestProp();
        }
    }
    
    /// <summary>
    /// Remove the prop to the player possible interactions list if it's existing.
    /// </summary>
    /// <param name="p">The Player concerned by the removal.</param>
    private void RemovePropToPlayerPossibleInteractions(PlayerNetwork p)
    {
        //Check if the prop is already in the list.
        if (p.allAvailableProps.Contains(this))
        {
            //Remove it because not interactable anymore.
            p.allAvailableProps.Remove(this);
            
            //Get the nearest prop and actualize it if necessary
            p.GetNearestProp();
        }
    }
}
