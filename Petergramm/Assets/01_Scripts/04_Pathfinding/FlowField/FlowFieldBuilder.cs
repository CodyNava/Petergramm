using System.Collections.Generic;
using _01_Scripts._02_Grid.GridData;
using _01_Scripts._08_GlobalManager.GridToEnemyConnector;
using NaughtyAttributes;
using UnityEngine;

namespace _01_Scripts._04_Pathfinding.FlowField
{
    public class FlowFieldBuilder : MonoBehaviour
    {
        [SerializeField] private GridData gridData;

        [SerializeField] private Vector3Int goalCoord;


        [Button]
        public void BuildFlowField()
        {
            gridData.ResetFlowData();
            GenerateTileCosts();
            GenerateFlowDirection();
            GridToEnemyConnector.SetGridPlacementCoords(gridData.PlacementCoords);
        }

        public void Start()
        {
            BuildFlowField();
        }

        private void GenerateTileCosts()
        {
            Queue<Vector3Int> flowFieldQueue = new();

            if (gridData.TryGetTileData(goalCoord, out var goalTileData))
            {
                goalTileData.costToGoal = 0;
                flowFieldQueue.Enqueue(goalCoord);
            }


            while (flowFieldQueue.Count > 0)
            {
                var currentCoord = flowFieldQueue.Dequeue();

                if (gridData.TryGetTileData(currentCoord, out var currentTileData))
                {
                    var currentCost = currentTileData.costToGoal;
                    foreach (var neighbour in gridData.GetNeighbours(currentCoord))
                    {
                        if (gridData.TryGetTileData(neighbour, out var neighbourTileData))
                        {
                            if (gridData.IsWalkable(neighbour))
                            {
                                int newCosts = currentCost + 1;

                                if (newCosts < neighbourTileData.costToGoal)
                                {
                                    neighbourTileData.costToGoal = newCosts;
                                    flowFieldQueue.Enqueue(neighbour);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void GenerateFlowDirection()
        {
            foreach ((Vector3Int key, GridTileData coord) in gridData.PlacementCoords)
            {
                if (!gridData.IsWalkable(key)
                    || coord.costToGoal == 0
                    || coord.costToGoal == int.MaxValue) continue;

                Vector3Int currentLowestCostNeighbor = Vector3Int.zero;
                int currentLowestCost = int.MaxValue;
                foreach (var neighbour in gridData.GetNeighbours(key))
                {
                    gridData.TryGetTileData(neighbour, out var neighbourTileData);
                    if (currentLowestCost > neighbourTileData.costToGoal
                        && neighbourTileData.costToGoal != int.MaxValue
                        && coord.costToGoal > neighbourTileData.costToGoal)
                    {
                        currentLowestCost = neighbourTileData.costToGoal;
                        currentLowestCostNeighbor = neighbour;
                    }
                }

                coord.flowDirection = currentLowestCostNeighbor - key;
            }
        }
    }
}