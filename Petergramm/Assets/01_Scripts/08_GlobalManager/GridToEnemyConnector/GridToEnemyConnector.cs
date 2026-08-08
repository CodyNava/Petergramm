using System.Collections.Generic;
using _01_Scripts._02_Grid.GridData;
using _01_Scripts._07_Enemy.Runtime;
using UnityEngine;

namespace _01_Scripts._08_GlobalManager.GridToEnemyConnector
{
   public static class GridToEnemyConnector
   {
      public static Vector3 GridStartPos;
      public static Dictionary<Vector3Int, GridTileData> GridPlacementCoords;

      public static void SetGridStartPos(this Vector3 gridStartPos) => GridStartPos = gridStartPos;

      public static Vector3Int ToGrid(this Vector3 worldPos) //extension method
      {
         var relativePos = worldPos - GridStartPos;

         return new Vector3Int(Mathf.RoundToInt(relativePos.x), 0, Mathf.RoundToInt(relativePos.z));
      }
      
      public static Vector3 ToWorld(this Vector3Int gridCoord) => GridStartPos + gridCoord;

      private static bool IsValidCoord(this Vector3Int coord) => GridPlacementCoords.ContainsKey(coord);
      private static bool IsUnreachableForEnemies(this Vector3Int coord)
      {
         if (GridPlacementCoords.TryGetValue(coord, out GridTileData tileData))
            return tileData.costToGoal == int.MaxValue;
         
         return true;
      }

      private static List<Vector3Int> GetNeighbors(this Vector3Int coord, int range)
      {
         var neighbors = new List<Vector3Int>();
         var vertical = new Vector3Int(0, 0, 1);
         var horizontal = new Vector3Int(1, 0, 0);

         void AddIfValid(Vector3Int a)
         {
            if (a.IsValidCoord() && !a.IsUnreachableForEnemies()) neighbors.Add(a);
         }
         AddIfValid(coord);

         for (var i = 1; i < range + 1; i++)
         {
            AddIfValid(coord + vertical * i);
            AddIfValid(coord - vertical * i);
            AddIfValid(coord + horizontal * i);
            AddIfValid(coord - horizontal * i);

            for (var j = 1; j < range + 1; j++)
            {
               AddIfValid(coord + vertical * i + horizontal * j);
               AddIfValid(coord + vertical * i - horizontal * j);
               AddIfValid(coord - vertical * i + horizontal * j);
               AddIfValid(coord - vertical * i - horizontal * j);
            }
         }

         return neighbors;
      }

      public static List<EnemyHealth> GetEnemiesInRange(this Vector3Int gridCoord, int range)
      {
         var enemiesInProximity = new List<EnemyHealth>();

         foreach (Vector3Int tile in gridCoord.GetNeighbors(range))
         {
            if (!TryGetTileData(tile, out GridTileData data)) continue;
            enemiesInProximity.AddRange(data.enemy);
         }

         return enemiesInProximity;
      }
      
      //GridData Usage
      public static void SetGridPlacementCoords
         (this Dictionary<Vector3Int, GridTileData> gridPlacementCoords) =>
         GridPlacementCoords = gridPlacementCoords;

      public static bool TryGetTileData(Vector3Int coord, out GridTileData tileData)
      {
         return GridPlacementCoords.TryGetValue(coord, out tileData);
      }

      public static Vector3Int LowestCostCoordToSpawn()
      {
         Vector3Int lowestCostCoord = Vector3Int.zero;
         int lowestCost = int.MaxValue;
         foreach (var (key, value) in GridPlacementCoords)
         {
            if (key.x > 1 || !IsValidCoord(key)) { continue; }

            if (lowestCost > value.costToGoal)
            {
               // Debug.Log($"New Lowest Cost Coord: {key}");
               // Debug.Log($"New Lowest Cost: {value.costToGoal}");
               lowestCost = value.costToGoal;
               lowestCostCoord = key;
            }
         }

         return lowestCostCoord;
      }
   }
}