using System;
using System.Collections.Generic;
using _01_Scripts._02_Grid.GridRendering;
using _01_Scripts._08_GlobalManager.GridToEnemyConnector;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace _01_Scripts._02_Grid.GridData
{
   public class GridData : MonoBehaviour
   {
      [SerializeField] private GridBase grid;
      [SerializeField] private Color color1;
      [SerializeField] private Color color2;
      [SerializeField] private float colorMultiply;

      [SerializedDictionary("Coord", "TileData")]
      public SerializedDictionary<Vector3Int, GridTileData> _placementCoords = new();

      public Dictionary<Vector3Int, GridTileData> PlacementCoords => _placementCoords;

      [Button]
      private void InitializeCoords()
      {
         _placementCoords.Clear();
         IReadOnlyList<Vector3Int> gridCoords = grid.SquareCoords;

         foreach (Vector3Int t in gridCoords) _placementCoords.Add(t, new GridTileData());
      }

      private bool IsValidCoord(Vector3Int coord) { return _placementCoords.ContainsKey(coord); }

      public bool IsWalkable(Vector3Int coord)
      {
         return _placementCoords.TryGetValue(coord, out GridTileData tileData) && !tileData.isOccupied;
      }

      public void ResetFlowData()
      {
         foreach (var (coord, tileData) in _placementCoords)
         {
            tileData.flowDirection = Vector3Int.zero;
            tileData.costToGoal = int.MaxValue;
         }
      }

      public bool TryGetTileData(Vector3Int coord, out GridTileData tileData)
      {
         return _placementCoords.TryGetValue(coord, out tileData);
      }

      public List<Vector3Int> GetNeighbours(Vector3Int coord)
      {
         List<Vector3Int> neighbours = new();
         Vector3Int neighbourCoord = Vector3Int.zero;

         // Upper
         neighbourCoord = coord + new Vector3Int(0, 0, 1);
         if (IsValidCoord(neighbourCoord)) neighbours.Add(neighbourCoord);

         // Down
         neighbourCoord = coord + new Vector3Int(0, 0, -1);
         if (IsValidCoord(neighbourCoord)) neighbours.Add(neighbourCoord);

         // Left
         neighbourCoord = coord + new Vector3Int(-1, 0, 0);
         if (IsValidCoord(neighbourCoord)) neighbours.Add(neighbourCoord);

         // Right
         neighbourCoord = coord + new Vector3Int(1, 0, 0);
         if (IsValidCoord(neighbourCoord)) neighbours.Add(neighbourCoord);

         return neighbours;
      }

      public void OnDrawGizmos()
      {
         foreach (var coord in _placementCoords)
         {
            var startPos = GridToEnemyConnector.GridStartPos;
            var coordFlow = coord.Value.costToGoal;
            Handles.color = Color.Lerp(color1, color2, coordFlow / colorMultiply);
            Handles.DrawSolidDisc(startPos + coord.Key, Vector3.up, 0.33f);
         }
      }
   }

   [Serializable]
   public class GridTileData
   {
      public bool isOccupied;
      public GameObject occupant;

      public int costToGoal = int.MaxValue;
      public Vector3Int flowDirection = Vector3Int.zero;
   }
}