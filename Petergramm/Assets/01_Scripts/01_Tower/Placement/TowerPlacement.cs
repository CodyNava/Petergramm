using System.Collections.Generic;
using _01_Scripts._02_Grid.GridData;
using _01_Scripts._04_Pathfinding.FlowField;
using _01_Scripts._08_GlobalManager.GridToEnemyConnector;
using _01_Scripts._09_Debugging;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _01_Scripts._01_Tower.Placement
{
    public class TowerPlacement : MonoBehaviour
    {
        private static TowerPlacement Instance { get; set; }
        [SerializeField] private List<GameObject> towerPrefab = new();
        [SerializeField] private GridData gridData;
        [SerializeField] private FlowFieldBuilder flowFieldBuilder;
        [SerializeField] private Camera cam;

        private static readonly ProfilerMarker TowerPlacementRayCast = new ProfilerMarker("TowerPlacementRayCast");

        private static readonly ProfilerMarker TowerPlacementPlacementSnapping =
            new ProfilerMarker("TowerPlacementPlacementSnapping");

        private static readonly ProfilerMarker
            TowerPlacementPlaceTower = new ProfilerMarker("TowerPlacementPlaceTower");

        private static readonly ProfilerMarker TowerPlacementDestroyTower =
            new ProfilerMarker("TowerPlacementDestroyTower");

        private bool _isDragging;
        private GameObject _draggingTower;
        private GridTileData _tile;
        private Vector3Int _gridCoord;
        
        
        //DEBUG
        [SerializeField] private DebugButtons debugButtons;
        //DEBUG

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

        private void Start()
        {
            cam = Camera.main;
        }

        private void Update()
        {
            RayForTowerPosition();

            if (_tile == null) return;

            PlaceTower();

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

                if (Physics.Raycast(ray, out RaycastHit hitDebug, 100f))
                   debugButtons.TestLightFollowMouse(hitDebug.point);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Grid")))
                {
                    
                    Vector3 hitPoint = hit.point;
                    _gridCoord = hitPoint.ToGrid(); //extension method

                    if (gridData.PlacementCoords.TryGetValue(_gridCoord, out GridTileData placementCoord))
                    {
                        //debugButtons.TestLightFollowMouseSnap(_gridCoord.ToWorld());
                        _tile = placementCoord;
                    }
                }
            }
        }

        private void PlaceTower()
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
                    if (_gridCoord.x < 1 || _gridCoord.x >= 23 || _gridCoord.z < 1 || _gridCoord.z >= 20) return;
                    
                    
                    var snapPosition = _gridCoord.ToWorld();

                    _draggingTower.transform.position = snapPosition;

                    if (Mouse.current.leftButton.isPressed)
                    {
                        PlaceTower(_gridCoord, snapPosition);
                    }
                }
            }
        }

        public void SpawnTower(string tower)
        {
            if (_draggingTower) DespawnTower();
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
                if (!gridData.PlacementCoords.TryGetValue(gridCoord, out GridTileData placementCoords) || placementCoords.costToGoal == 0) return;
                _draggingTower.transform.position = snapPosition;
                placementCoords.isOccupied = true;
                placementCoords.occupant = _draggingTower;

                //todo energy needs to increase based on towers energy stat
                //todo and a energy global stat is needed

                flowFieldBuilder.BuildFlowField();

                if (Keyboard.current.leftShiftKey.isPressed)
                    _draggingTower = Instantiate(_draggingTower, Vector3.zero, Quaternion.identity);

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
                if (!gridData.PlacementCoords.TryGetValue(gridCoord, out GridTileData placementCoords)) return;
                if (!placementCoords.isOccupied) return;
                Destroy(placementCoords.occupant.gameObject);
                placementCoords.isOccupied = false;
                placementCoords.occupant = null;
                flowFieldBuilder.BuildFlowField();
            }
        }
    }
}