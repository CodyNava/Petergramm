using _01_Scripts._02_Grid.GridData;
using _01_Scripts._02_Grid.GridRendering;
using UnityEngine;

namespace _01_Scripts._07_Enemy.Runtime
{
    public class EnemyFlowMovement : MonoBehaviour
    {
        [SerializeField] private EnemyRuntime enemyRuntime;
        [SerializeField] private GridData gridData;
        [SerializeField] private GridBase gridBase;

        private Vector3 _currentTargetWorldPosition;

        private void Start()
        {
            gridBase.WorldToGrid(transform.position, out var gridCoord);
            gridBase.GridToWorld(gridCoord, out _currentTargetWorldPosition);
            _currentTargetWorldPosition.y = transform.position.y;
        }

        private void Update()
        {
            MoveTowardsTarget();
        }

        private void SetNextTarget()
        {
            gridBase.WorldToGrid(transform.position, out var currentCoord);
            if (gridData.TryGetTileData(currentCoord, out var tileData))
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