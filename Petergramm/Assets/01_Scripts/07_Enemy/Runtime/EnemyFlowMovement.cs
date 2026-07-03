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

        private Vector3Int _flowDirection;

        private void Update()
        {
            gridBase.WorldToGrid(transform.position, out var gridCoord);
            if (gridData.TryGetTileData(gridCoord, out var tileData))
            {
                _flowDirection = tileData.flowDirection;

                if (_flowDirection != Vector3Int.zero)
                {
                    Vector3 moveDir = new Vector3(_flowDirection.x, 0, _flowDirection.z);


                    transform.position += moveDir.normalized *
                                          (enemyRuntime.CurrentStats.movement.moveSpeed * Time.deltaTime);
                }
            }
        }
    }
}