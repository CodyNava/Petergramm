using System.Collections.Generic;
using _01_Scripts._02_Grid.GridData;
using NaughtyAttributes;
using UnityEngine;

namespace _01_Scripts._04_Pathfinding.FlowFieldBuilder
{
    public class FlowFieldBuilder : MonoBehaviour
    {
        [SerializeField] private GridData gridData;

        [SerializeField] private Vector3Int goalCoord;


        [Button]
        public void BuildFlowField()
        {
            Queue<Vector3Int> flowFieldQueue = new();

            gridData.ResetFlowData();

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
    }
}