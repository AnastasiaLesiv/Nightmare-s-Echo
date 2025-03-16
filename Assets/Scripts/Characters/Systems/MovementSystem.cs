using Leopotam.Ecs;
using UnityEngine;

namespace Characters.Systems
{
    public class MovementSystem : IEcsRunSystem
    {
        private readonly EcsWorld _world = null;
        private readonly EcsFilter<PlayerTag, PositionComponent, DirectionComponent, MovableComponent> _movableFilter = null;
        private readonly EcsFilter<MapDataComponent> _mapFilter = null;
        
        private const float MoveSpeed = 5f;

        public void Run()
        {
            foreach (var i in _movableFilter)
            {
                ref var positionComponent = ref _movableFilter.Get2(i);
                ref var directionComponent = ref _movableFilter.Get3(i);
                ref var movableComponent = ref _movableFilter.Get4(i);

                Vector2 direction = directionComponent.Direction;
                if (direction == Vector2.zero) continue;

                GameObject playerObj = _movableFilter.GetEntity(i).Get<PlayerTag>().GameObject;
                if (playerObj == null)
                {
                    Debug.LogError($"Player GameObject is null for entity {i}");
                    continue;
                }

                // Розраховуємо нову позицію з урахуванням швидкості та часу
                Vector3 currentPosition = playerObj.transform.position;
                Vector3 movement = new Vector3(direction.x, direction.y, 0) * (MoveSpeed * Time.deltaTime);
                Vector3 newPosition = currentPosition + movement;

                // Перевіряємо, чи нова позиція валідна
                if (IsValidPosition(newPosition))
                {
                    playerObj.transform.position = newPosition;
                    positionComponent.x = Mathf.RoundToInt(newPosition.x);
                    positionComponent.y = Mathf.RoundToInt(newPosition.y);
                }
            }
        }

        private bool IsValidPosition(Vector3 position)
        {
            int x = Mathf.RoundToInt(position.x);
            int y = Mathf.RoundToInt(position.y);

            foreach (var i in _mapFilter)
            {
                ref var mapData = ref _mapFilter.Get1(i);
                if (x < 0 || x >= mapData.Width || y < 0 || y >= mapData.Height)
                {
                    return false;
                }

                var tileEntity = mapData.TileEntities[x, y];
                if (tileEntity.IsAlive() && tileEntity.Has<MapTileComponent>())
                {
                    ref var tile = ref tileEntity.Get<MapTileComponent>();
                    return tile.TileType == 1;
                }
            }
            return false;
        }
    }
}