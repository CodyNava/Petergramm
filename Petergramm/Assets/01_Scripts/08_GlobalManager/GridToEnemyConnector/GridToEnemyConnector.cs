using System.Collections.Generic;
using _01_Scripts._02_Grid.GridData;
using _01_Scripts._02_Grid.GridRendering;
using UnityEngine;

namespace _01_Scripts._08_GlobalManager.GridToEnemyConnector
{
    public static class GridToEnemyConnector
    {
        public static Vector3 GridStartPos;
        public static Dictionary<Vector3Int, GridTileData> GridPlacementCoords;
        
        //GridBase Usage
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
        
        //GridData Usage
        public static void SetGridPlacementCoords(Dictionary<Vector3Int, GridTileData> gridPlacementCoords) 
            => GridPlacementCoords = gridPlacementCoords;
        
        public static bool TryGetTileData(Vector3Int coord, out GridTileData tileData)
        {
            return GridPlacementCoords.TryGetValue(coord, out tileData);
        }

    }
}
