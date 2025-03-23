using Leopotam.Ecs;
using UnityEngine;

namespace Characters.Systems
{
    public class CameraFollowSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorld _world = null;
        private readonly EcsFilter<PlayerTag> _playerFilter = null;
        private readonly EcsFilter<MapDataComponent> _mapFilter = null;

        private Camera _mainCamera;
        private Vector3 _offset = new Vector3(0, 0, -10);
        
        private float _minZoom = 3f;
        private float _maxZoom = 8f;
        private float _currentZoom = 5f;
        private float _zoomSpeed = 1f;
        private float _followSpeed = 10f;
        
        private Vector2 _mapMin;
        private Vector2 _mapMax;
        private float _cameraHalfHeight;
        private float _cameraHalfWidth;
        private float _padding = 1f;

        public void Init()
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null) return;

            _mainCamera.orthographicSize = _currentZoom;

            foreach (var i in _mapFilter)
            {
                ref var mapData = ref _mapFilter.Get1(i);
                _mapMin = new Vector2(0, 0);
                _mapMax = new Vector2(mapData.Width - 1, mapData.Height - 1);
                break;
            }
        }

        public void Run()
        {
            if (_mainCamera == null) return;

            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput != 0)
            {
                _currentZoom = Mathf.Clamp(_currentZoom - scrollInput * _zoomSpeed, _minZoom, _maxZoom);
                _mainCamera.orthographicSize = _currentZoom;
            }

            _cameraHalfHeight = _mainCamera.orthographicSize;
            _cameraHalfWidth = _cameraHalfHeight * _mainCamera.aspect;

            foreach (var i in _playerFilter)
            {
                GameObject playerObj = _playerFilter.Get1(i).GameObject;
                if (playerObj != null)
                {
                    Vector3 playerPosition = playerObj.transform.position;
                    Vector3 targetPosition = playerPosition + _offset;

                    float minX = _mapMin.x + _cameraHalfWidth - _padding;
                    float maxX = _mapMax.x - _cameraHalfWidth + _padding;
                    float minY = _mapMin.y + _cameraHalfHeight - _padding;
                    float maxY = _mapMax.y - _cameraHalfHeight + _padding;

                    targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
                    targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

                    if (playerPosition.x < minX + _padding) targetPosition.x = minX;
                    if (playerPosition.x > maxX - _padding) targetPosition.x = maxX;
                    if (playerPosition.y < minY + _padding) targetPosition.y = minY;
                    if (playerPosition.y > maxY - _padding) targetPosition.y = maxY;

                    _mainCamera.transform.position = Vector3.Lerp(
                        _mainCamera.transform.position,
                        new Vector3(targetPosition.x, targetPosition.y, _offset.z),
                        Time.deltaTime * _followSpeed
                    );
                }
            }
        }
    }
} 