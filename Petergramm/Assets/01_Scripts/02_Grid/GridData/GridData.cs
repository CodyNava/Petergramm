using System;
using System.Collections.Generic;
using _01_Scripts._02_Grid.GridRendering;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEngine;

namespace _01_Scripts._02_Grid.GridData
{
   public class GridData : MonoBehaviour
   {

      [SerializeField] private GridBase grid;
      [SerializedDictionary("Coord", "TileData")] public SerializedDictionary<Vector3, GridTileData> placementCoords = new();
      
      public Dictionary<Vector3, GridTileData> PlacementCoords => placementCoords;

      [Button]
      private void InitializeCoords()
      {
         placementCoords.Clear();
         IReadOnlyList<Vector3> gridCoords = grid.SquareCoords;
         
         foreach (Vector3 t in gridCoords) 
            placementCoords.Add(t, new GridTileData());

         foreach (KeyValuePair<Vector3, GridTileData> pair in placementCoords)
            pair.Value.coords = pair.Key;
        
      }
      
   }

   [Serializable]
   public class GridTileData
   {
      public bool isOccupied;
      public GameObject occupant;
      public Vector3 coords;
   }
}
