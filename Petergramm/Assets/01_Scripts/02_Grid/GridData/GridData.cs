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

        [SerializedDictionary("Coord", "TileData")]
        public SerializedDictionary<Vector3Int, GridTileData> placementCoords = new();

        public Dictionary<Vector3Int, GridTileData> PlacementCoords => placementCoords;

        [Button]
        private void InitializeCoords()
        {
            placementCoords.Clear();
            IReadOnlyList<Vector3Int> gridCoords = grid.SquareCoords;

            foreach (Vector3Int t in gridCoords)
                placementCoords.Add(t, new GridTileData());
        }

        public bool IsValidCoord(Vector3Int coord)
        {
            return placementCoords.ContainsKey(coord);
        }

        public bool IsWalkable(Vector3Int coord)
        {
            return placementCoords.TryGetValue(coord, out GridTileData tileData)
                   && !tileData.isOccupied;
        }

        public void ResetFlowData()
        {
            foreach (var (coord, tileData) in placementCoords)
            {
                tileData.flowDirection = Vector3Int.zero;
                tileData.costToGoal = int.MaxValue;
            }
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
            foreach (var coord in placementCoords)
            {
                var newColor = new Color(0.1f , 0.1f, 0.1f, 0.1f);
                var coordFlow = coord.Value.costToGoal;
                Gizmos.DrawWireCube(coord.Key + new Vector3Int(4,0,0), Vector3.one);
                Gizmos.color = newColor * coordFlow;
                
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