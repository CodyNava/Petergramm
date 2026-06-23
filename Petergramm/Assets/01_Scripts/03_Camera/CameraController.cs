using _01_Scripts._05_InputSystem;
using UnityEngine;

namespace _01_Scripts._03_Camera
{
    public class CameraController : MonoBehaviour
    {
        private Camera _camera;
        
        [Header("Zoom")]
        [SerializeField] private float zoomSpeed;
        [SerializeField] private float minZoom = 4f;
        [SerializeField] private float maxZoom = 13f;

        private float _currentZoom;
        
        [Header("Panning")]
        [SerializeField] private float panningSpeed;
        
        private void Awake()
        {
            _camera = Camera.main;
            _currentZoom = Mathf.Clamp(_camera!.orthographicSize, minZoom, maxZoom);
            _camera.orthographicSize = _currentZoom;
        }

        private void Update()
        {
            HandlePanning();
            HandleZoom();
        }

        private void HandlePanning()
        {
            var isDragging = InputManager.Input.Camera.DragHold.IsPressed();

            if (!isDragging)
                return;
            var mouseDelta = InputManager.Input.Camera.MouseDelta.ReadValue<Vector2>();
            var move = new Vector3(-mouseDelta.x, -mouseDelta.y, 0f);
            _camera.transform.position += move * panningSpeed * Time.deltaTime;
        }

        private void HandleZoom()
        {
            var mousePos = InputManager.Input.Camera.MousePos.ReadValue<Vector2>();
            var zoomInput = InputManager.Input.Camera.Zoom.ReadValue<Vector2>().y;
            
            var worldBefore = _camera.ScreenToWorldPoint(mousePos);
            _currentZoom = Mathf.Clamp(_currentZoom - zoomInput * zoomSpeed, minZoom, maxZoom);
            
            _camera.orthographicSize = _currentZoom;
            var worldAfter = _camera.ScreenToWorldPoint(mousePos);
            var offset = worldBefore - worldAfter;
            _camera.transform.position += offset;
        }
    }
}