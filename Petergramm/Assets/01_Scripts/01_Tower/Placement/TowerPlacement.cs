using System.Collections.Generic;
using _01_Scripts._02_Grid.GridData;
using _01_Scripts._04_Pathfinding.FlowField;
using _01_Scripts._08_GlobalManager.GridToEnemyConnector;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _01_Scripts._01_Tower.Placement
{
   public class TestTowerPlacement : MonoBehaviour
   {
      public static TestTowerPlacement Instance { get; private set; }
      [SerializeField] private List<GameObject> towerPrefab = new();
      [SerializeField] private GridData gridData;
      [SerializeField] private FlowFieldBuilder flowFieldBuilder;
      [SerializeField] private Camera cam;

      private static readonly ProfilerMarker TowerPlacementRayCast = new ProfilerMarker("TowerPlacementRayCast");
      private static readonly ProfilerMarker TowerPlacementPlacementSnapping =
         new ProfilerMarker("TowerPlacementPlacementSnapping");
      private static readonly ProfilerMarker TowerPlacementPlaceTower = new ProfilerMarker("TowerPlacementPlaceTower");
      private static readonly ProfilerMarker TowerPlacementDestroyTower =
         new ProfilerMarker("TowerPlacementDestroyTower");

      private bool _isDragging;
      private GameObject _draggingTower;
      private GridTileData _tile;
      private Vector3Int _gridCoord;

      private void Awake()
      {
         if (Instance == null)
         {
            Instance = this;
            DontDestroyOnLoad(gameObject);
         }
         else
         {
            Destroy(gameObject);
         }
      }

      private void Start() { cam = Camera.main; }

      private void Update()
      {
         RayForTowerPosition();

         if (_tile == null) return;

         TowerPlacement();

         //TowerDestroy

         if (Mouse.current.rightButton.isPressed)
         {
            if (_isDragging) return;
            DestroyTower(_gridCoord);
         }
      }

      private void RayForTowerPosition()
      {
         using (TowerPlacementRayCast.Auto())
         {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            if (!cam) return;

            Ray ray = cam.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Grid")))
            {
               Vector3 hitPoint = hit.point;
               //int x = Mathf.RoundToInt(hitPoint.x);
               //int z = Mathf.RoundToInt(hitPoint.z);

               GridToEnemyConnector.WorldToGrid(hitPoint, out _gridCoord);

               //_gridCoord = new Vector3Int(x, 0, z);

               if (gridData.placementCoords.ContainsKey(_gridCoord)) { _tile = gridData.placementCoords[_gridCoord]; }
            }
         }
      }

      private void TowerPlacement()
      {
         using (TowerPlacementPlacementSnapping.Auto())
         {
            if (!_isDragging) return;

            if (Mouse.current.rightButton.isPressed)
            {
               DespawnTower();
               return;
            }

            if (!_tile.isOccupied)
            {
               GridToEnemyConnector.GridToWorld(_gridCoord, out Vector3 snapPosition);
               _draggingTower.transform.position = snapPosition;

               if (Mouse.current.leftButton.isPressed) { PlaceTower(_gridCoord, snapPosition); }
            }
         }
      }

      public void SpawnTower(string tower)
      {
         var towerToSpawn = towerPrefab.Find(t => t.name == tower);
         _draggingTower = Instantiate(towerToSpawn, Vector3.zero, Quaternion.identity);
         _isDragging = true;
      }

      private void DespawnTower()
      {
         if (!_draggingTower) return;
         Destroy(_draggingTower);
         _draggingTower = null;
         _isDragging = false;
      }

      private void PlaceTower(Vector3Int gridCoord, Vector3 snapPosition)
      {
         using (TowerPlacementPlaceTower.Auto())
         {
            if (!gridData.placementCoords.ContainsKey(gridCoord)) return;
            var placementCoords = gridData.PlacementCoords[gridCoord];
            _draggingTower.transform.position = snapPosition;
            placementCoords.isOccupied = true;
            placementCoords.occupant = _draggingTower;

            //todo energy needs to increase based on towers energy stat
            //todo and a energy global stat is needed

            flowFieldBuilder.BuildFlowField();

            if (Keyboard.current.leftShiftKey.isPressed)
               _draggingTower = Instantiate(towerPrefab[0], Vector3.zero, Quaternion.identity);

            if (!Keyboard.current.leftShiftKey.isPressed)
            {
               _isDragging = false;
               _draggingTower = null;
            }
         }
      }

      private void DestroyTower(Vector3Int gridCoord)
      {
         using (TowerPlacementDestroyTower.Auto())
         {
            if (!gridData.placementCoords.ContainsKey(gridCoord)) return;
            var placementCoords = gridData.PlacementCoords[gridCoord];
            if (!placementCoords.isOccupied) return;
            Destroy(placementCoords.occupant.gameObject);
            placementCoords.isOccupied = false;
            placementCoords.occupant = null;
            flowFieldBuilder.BuildFlowField();
         }
      }
   }
}