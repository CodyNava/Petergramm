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
      [SerializedDictionary("Coord", "TileData")] public SerializedDictionary<Vector3Int, GridTileData> placementCoords = new();
      
      public Dictionary<Vector3Int, GridTileData> PlacementCoords => placementCoords;

      [Button]
      private void InitializeCoords()
      {
         placementCoords.Clear();
         IReadOnlyList<Vector3Int> gridCoords = grid.SquareCoords;
         
         foreach (Vector3Int t in gridCoords) 
            placementCoords.Add(t, new GridTileData());
      }
      
   }

   [Serializable]
   public class GridTileData
   {
      public bool isOccupied;
      public GameObject occupant;
   }
}
