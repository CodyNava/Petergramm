using System;
using _01_Scripts._02_Grid.GridData;
using _01_Scripts._08_GlobalManager.EnemyList;
using _01_Scripts._08_GlobalManager.GridToEnemyConnector;
using Unity.Profiling;
using UnityEngine;

namespace _01_Scripts._07_Enemy.Runtime
{
   public class EnemyFlowMovement : MonoBehaviour
   {
      [SerializeField] private EnemyRuntime enemyRuntime;
      [SerializeField] private EnemyHealth enemyHealth;
      private Vector3 _currentTargetWorldPosition;
      private Vector3Int _oldPos;

      private static readonly ProfilerMarker EnemyFlowWalkTowardsSpawnTarget =
         new ProfilerMarker("EnemyFlowWalkTowardsSpawnTarget");
      private static readonly ProfilerMarker EnemyFlowSetNextTarget = new ProfilerMarker("EnemyFlowSetNextTarget");
      private static readonly ProfilerMarker EnemyFlowMoveTowardsTarget =
         new ProfilerMarker("EnemyFlowMoveTowardsTarget");

      private void Start()
      {
         SetSpawnTile();
         RefreshEnemyTilePos(this.transform.position.ToGrid());
      }

      private void OnEnable() { SetSpawnTile(); }

      private void Update() { MoveTowardsTarget(); }

      private void SetSpawnTile()
      {
         using (EnemyFlowWalkTowardsSpawnTarget.Auto())
         {
            var gridCoord = transform.position.ToGrid();
            _oldPos = gridCoord;
            _currentTargetWorldPosition = gridCoord.ToWorld();
            _currentTargetWorldPosition.y = transform.position.y;
         }
      }

      private void RefreshEnemyTilePos(Vector3Int pos)
      {
         if (GridToEnemyConnector.GridPlacementCoords.TryGetValue(_oldPos, out GridTileData oldCoord))
            oldCoord.enemy.Remove(this.enemyHealth);

         _oldPos = pos;

         if (GridToEnemyConnector.GridPlacementCoords.TryGetValue(pos, out GridTileData coord))
            coord.enemy.Add(this.enemyHealth);
      }

      private void SetNextTarget()
      {
         using (EnemyFlowSetNextTarget.Auto())
         {
            var currentCoord = transform.position.ToGrid();
            RefreshEnemyTilePos(currentCoord);
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
            //if (transform.position == _currentTargetWorldPosition) { SetNextTarget(); }
         }
      }
   }
}