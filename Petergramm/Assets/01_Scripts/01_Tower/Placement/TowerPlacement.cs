using System.Collections.Generic;
using _01_Scripts._02_Grid.GridData;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _01_Scripts._01_Tower.Placement
{
    public class TestTowerPlacement : MonoBehaviour
    {
        [SerializeField] private List<GameObject> towerPrefab = new();
        [SerializeField] private GridData gridData;
        [SerializeField] private Camera cam;


        private bool _isDragging;
        private GameObject _draggingTower;
        private GridTileData _tile;
        private Vector3Int _gridCoord = Vector3Int.zero;

        private void Update()
        {
            RayForTowerPosition();

            if (_tile == null) return;

            TowerPlacement();

            //TowerDestroy
            if (_tile.isOccupied && Mouse.current.rightButton.isPressed && Keyboard.current.leftShiftKey.isPressed)
            {
                DestroyTower(_gridCoord);
            }
        }

        private void RayForTowerPosition()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            if (!cam) return;
            
            Ray ray = cam.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Grid")))
            {
                Vector3 hitPoint = hit.point;
                int x = Mathf.RoundToInt(hitPoint.x);
                int z = Mathf.RoundToInt(hitPoint.z);

                _gridCoord = new Vector3Int(x, 0, z);

                if (gridData.placementCoords.ContainsKey(_gridCoord))
                {
                    _tile = gridData.placementCoords[_gridCoord];
                }
            }
        }

        private void TowerPlacement()
        {
            if (!_isDragging) return;

            if (Mouse.current.rightButton.isPressed)
            {
                DespawnTower();
            }

            if (!_tile.isOccupied)
            {
                Vector3 snapPosition = new Vector3(_gridCoord.x, _gridCoord.y, _gridCoord.z);
                _draggingTower.transform.position = snapPosition;

                if (Mouse.current.leftButton.isPressed)
                {
                    PlaceTower(_gridCoord);
                }
            }
        }

        public void SpawnTower()
        {
            _draggingTower = Instantiate(towerPrefab[0], Vector3.zero, Quaternion.identity);
            _isDragging = true;
        }

        private void DespawnTower()
        {
            Destroy(_draggingTower);
            _draggingTower = null;
            _isDragging = false;
        }


        private void PlaceTower(Vector3Int gridCoord)
        {
            var placementCoords = gridData.PlacementCoords[gridCoord];
            _draggingTower.transform.position = gridCoord;
            placementCoords.isOccupied = true;
            placementCoords.occupant = _draggingTower;

            if (!Keyboard.current.leftShiftKey.isPressed)
            {
                _isDragging = false;
                _draggingTower = null;
                return;
            }

            _draggingTower = Instantiate(towerPrefab[0], Vector3.zero, Quaternion.identity);
        }

        private void DestroyTower(Vector3Int gridCoord)
        {
            var placementCoords = gridData.PlacementCoords[gridCoord];
            Destroy(placementCoords.occupant.gameObject);
            placementCoords.isOccupied = false;
            placementCoords.occupant = null;
        }
    }
}