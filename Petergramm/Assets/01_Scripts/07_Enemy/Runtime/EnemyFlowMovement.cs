using System;
using _01_Scripts._08_GlobalManager.GridToEnemyConnector;
using Unity.Profiling;
using UnityEngine;

namespace _01_Scripts._07_Enemy.Runtime
{
    public class EnemyFlowMovement : MonoBehaviour
    {
        [SerializeField] private EnemyRuntime enemyRuntime;
        private Vector3 _currentTargetWorldPosition;

        private static readonly ProfilerMarker EnemyFlowWalkTowardsSpawnTarget =
            new ProfilerMarker("EnemyFlowWalkTowardsSpawnTarget");

        private static readonly ProfilerMarker EnemyFlowSetNextTarget = new ProfilerMarker("EnemyFlowSetNextTarget");

        private static readonly ProfilerMarker EnemyFlowMoveTowardsTarget =
            new ProfilerMarker("EnemyFlowMoveTowardsTarget");

        private void Start()
        {
            SetSpawnTile();
        }

        private void OnEnable()
        {
            SetSpawnTile();
        }
        
        private void Update()
        {
            MoveTowardsTarget();
        }


        //todo WalkTowardsSpawnTarget() ist extrem teuer, anhand profilings
        //todo wir müssen die grid/flowmap für gegner vergrößern und als spawnpoint/area nutzen
        private void SetSpawnTile()
        {
            using (EnemyFlowWalkTowardsSpawnTarget.Auto())
            {
                var gridCoord = transform.position.ToGrid();
                _currentTargetWorldPosition = gridCoord.ToWorld();
                _currentTargetWorldPosition.y = transform.position.y;
            }
        }

        private void SetNextTarget()
        {
            using (EnemyFlowSetNextTarget.Auto())
            {
                var currentCoord = transform.position.ToGrid();
                if (GridToEnemyConnector.TryGetTileData(currentCoord, out var tileData))
                {
                    var flowDirection = tileData.flowDirection;
                    if (flowDirection != Vector3Int.zero)
                    {
                        var nextTarget = transform.position + flowDirection;
                        nextTarget.y = transform.position.y;
                        _currentTargetWorldPosition = nextTarget;
                    }
                }
            }
        }

        private void MoveTowardsTarget()
        {
            using (EnemyFlowMoveTowardsTarget.Auto())
            {
                var moveDistance = enemyRuntime.CurrentStats.movement.moveSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, _currentTargetWorldPosition, moveDistance);
                transform.LookAt(_currentTargetWorldPosition);
                if (Vector3.Distance(transform.position, _currentTargetWorldPosition) < 0.1f)
                {
                    SetNextTarget();
                }
            }
        }
    }
}