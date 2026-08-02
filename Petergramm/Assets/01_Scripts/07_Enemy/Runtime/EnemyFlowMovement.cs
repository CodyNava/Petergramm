using _01_Scripts._02_Grid.GridData;
using _01_Scripts._08_GlobalManager.GridToEnemyConnector;
using UnityEngine;

namespace _01_Scripts._07_Enemy.Runtime
{
    public class EnemyFlowMovement : MonoBehaviour
    {
        [SerializeField] private EnemyRuntime enemyRuntime;

        private Vector3 _currentTargetWorldPosition;

        private bool _reachedSpawnTarget = false;

        private void Start()
        {
            //GridToEnemyConnector.WorldToGrid(transform.position, out var gridCoord);
        }

        private void Update()
        {
            if (!_reachedSpawnTarget)
            {
                GridToEnemyConnector.GridToWorld(GridToEnemyConnector.LowestCostCoordToSpawn(),
                    out _currentTargetWorldPosition);
                _currentTargetWorldPosition.y = transform.position.y;
                if (transform.position == _currentTargetWorldPosition)
                {
                    _reachedSpawnTarget = true;
                }
            }

            MoveTowardsTarget();
        }

        private void SetNextTarget()
        {
            GridToEnemyConnector.WorldToGrid(transform.position, out var currentCoord);
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

        private void MoveTowardsTarget()
        {
            var moveDistance = enemyRuntime.CurrentStats.movement.moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _currentTargetWorldPosition, moveDistance);
            if (Vector3.Distance(transform.position, _currentTargetWorldPosition) < 0.1f)
            {
                SetNextTarget();
            }
        }
    }
}