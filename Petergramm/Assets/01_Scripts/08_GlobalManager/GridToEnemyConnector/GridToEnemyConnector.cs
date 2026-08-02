using System.Collections.Generic;
using _01_Scripts._02_Grid.GridData;
using UnityEngine;

namespace _01_Scripts._08_GlobalManager.GridToEnemyConnector
{
    public static class GridToEnemyConnector
    {
        public static Vector3 GridStartPos;
        public static Dictionary<Vector3Int, GridTileData> GridPlacementCoords;

        public static void SetGridStartPos(Vector3 gridStartPos)
            => GridStartPos = gridStartPos;

        public static void WorldToGrid(Vector3 worldPos, out Vector3Int gridCoord)
        {
            var relativePos = worldPos - GridStartPos;

            gridCoord = new Vector3Int(
                Mathf.RoundToInt(relativePos.x),
                0,
                Mathf.RoundToInt(relativePos.z)
            );
        }

        public static void GridToWorld(Vector3Int gridCoord, out Vector3 worldPos)
        {
            worldPos = GridStartPos + gridCoord;
        }

        private static bool IsValidCoord(Vector3Int coord)
        {
            return GridPlacementCoords.ContainsKey(coord);
        }

        //GridData Usage
        public static void SetGridPlacementCoords(Dictionary<Vector3Int, GridTileData> gridPlacementCoords)
            => GridPlacementCoords = gridPlacementCoords;

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
                if (key.x > 1 || !IsValidCoord(key))
                {
                    continue;
                }

                if (lowestCost > value.costToGoal)
                {
                    Debug.Log($"New Lowest Cost Coord: {key}");
                    Debug.Log($"New Lowest Cost: {value.costToGoal}");
                    lowestCost = value.costToGoal;
                    lowestCostCoord = key;
                }
            }

            return lowestCostCoord;
        }
    }
}