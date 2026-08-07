using _01_Scripts._08_GlobalManager.GridToEnemyConnector;
using Unity.Profiling;
using UnityEngine;

namespace _01_Scripts._07_Enemy.Runtime
{
   public class EnemyFlowMovement : MonoBehaviour
   {
      [SerializeField] private EnemyRuntime enemyRuntime;
      private Vector3 _currentTargetWorldPosition;
      private bool _reachedSpawnTarget = false;

      private static readonly ProfilerMarker EnemyFlowWalkTowardsSpawnTarget = new ProfilerMarker("EnemyFlowWalkTowardsSpawnTarget");
      private static readonly ProfilerMarker EnemyFlowSetNextTarget = new ProfilerMarker("EnemyFlowSetNextTarget");
      private static readonly ProfilerMarker EnemyFlowMoveTowardsTarget =
         new ProfilerMarker("EnemyFlowMoveTowardsTarget");

      private void Start()
      {
         //GridToEnemyConnector.WorldToGrid(transform.position, out var gridCoord);
      }

      private void OnEnable() { _reachedSpawnTarget = false; }

      private void Update()
      {
         WalkTowardsSpawnTarget();
         MoveTowardsTarget();
      }

      
      //todo WalkTowardsSpawnTarget() ist extrem teuer, anhand profilings
      //todo wir müssen die grid/flowmap für gegner vergrößern und als spawnpoint/area nutzen
      private void WalkTowardsSpawnTarget()
      {
         
         using (EnemyFlowWalkTowardsSpawnTarget.Auto())
         {
            if (!_reachedSpawnTarget)
            {
               _currentTargetWorldPosition = GridToEnemyConnector.LowestCostCoordToSpawn().ToWorld();
               _currentTargetWorldPosition.y = transform.position.y;
               if (transform.position == _currentTargetWorldPosition) { _reachedSpawnTarget = true; }
            }
         }
      }

      private void SetNextTarget()
      {
         using (EnemyFlowSetNextTarget.Auto())
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
      }

      private void MoveTowardsTarget()
      {
         using (EnemyFlowMoveTowardsTarget.Auto())
         {
            var moveDistance = enemyRuntime.CurrentStats.movement.moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _currentTargetWorldPosition, moveDistance);
            transform.LookAt(_currentTargetWorldPosition);
            if (Vector3.Distance(transform.position, _currentTargetWorldPosition) < 0.1f) { SetNextTarget(); }
         }
      }
   }
}