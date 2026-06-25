using System.Collections.Generic;
using _01_Scripts._02_Grid.GridData;
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
        }

        public void GenerateTileCosts()
        {
            Queue<Vector3Int> flowFieldQueue = new();

            if (gridData.TryGetTileData(goalCoord, out var goalTileData))
            {
                goalTileData.costToGoal = 0;
                flowFieldQueue.Enqueue(goalCoord);
            }


            Vector3Int currentCoord;
            int currentCost;

            while (flowFieldQueue.Count > 0)
            {
                currentCoord = flowFieldQueue.Dequeue();

                if (gridData.TryGetTileData(currentCoord, out var currentTileData))
                {
                    currentCost = currentTileData.costToGoal;
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

        public void GenerateFlowDirection()
        {
            foreach (var coord in gridData.placementCoords)
            {
                if (!gridData.IsWalkable(coord.Key)
                    || coord.Value.costToGoal == 0
                    || coord.Value.costToGoal == int.MaxValue) continue;

                Vector3Int currentLowestCostNeighbor = Vector3Int.zero;
                int currentLowestCost = int.MaxValue;
                foreach (var neighbour in gridData.GetNeighbours(coord.Key))
                {
                    gridData.TryGetTileData(neighbour, out var neighbourTileData);
                    if (currentLowestCost > neighbourTileData.costToGoal
                        && neighbourTileData.costToGoal != int.MaxValue
                        && coord.Value.costToGoal > neighbourTileData.costToGoal)
                    {
                        currentLowestCost = neighbourTileData.costToGoal;
                        currentLowestCostNeighbor = neighbour;
                    }
                }

                coord.Value.flowDirection = currentLowestCostNeighbor - coord.Key;
            }
        }
    }
}