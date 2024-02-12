using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerList : MonoBehaviour
{
   public List<PlayerNetwork> players = new List<PlayerNetwork>();
   public static PlayerList Singleton;
   
   private void Awake()
   {
      Singleton = this;
   }

   public void AddPlayerToList(PlayerNetwork p)
   {
      players.Add(p);
   }
   
   public void RemovePlayerToList(PlayerNetwork p)
   {
      players.Remove(p);
   }

   public bool IsPlayerExist(PlayerNetwork p)
   {
      return players.Contains(p);
   }

   public bool IsAPlayer<T>(T objectType) where T : Component
   {
      return objectType.GetComponent<PlayerNetwork>();
   }
}
