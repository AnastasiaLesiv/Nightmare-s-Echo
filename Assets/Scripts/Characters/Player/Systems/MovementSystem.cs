using Leopotam.Ecs;
using UnityEngine;

namespace Characters.Systems
{
    public class MovementSystem : IEcsRunSystem
    {
        private readonly EcsWorld _world = null;
        private readonly EcsFilter<PlayerTag, PositionComponent, DirectionComponent, MovableComponent> _movableFilter = null;
        private readonly EcsFilter<MapDataComponent> _mapFilter = null;
        private readonly EcsFilter<GameStateComponent> _gameStateFilter = null;
        
        private const float MoveSpeed = 3f;

        public void Run()
        {
            if (_mapFilter.IsEmpty()) return;
            if (_gameStateFilter.IsEmpty()) return;
            
            ref var gameState = ref _gameStateFilter.Get1(0);
            if (gameState.CurrentMode != GameMode.Exploration) return;
            
            ref var mapData = ref _mapFilter.Get1(0);
            
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

                // Перевіряємо границі карти
                int newX = Mathf.RoundToInt(newPosition.x);
                int newY = Mathf.RoundToInt(newPosition.y);

                // Змінюємо перевірку границь, щоб дозволити рух по всій карті
                if (newX >= 0 && newX < mapData.Width && newY >= 0 && newY < mapData.Height)
                {
                    // Перевіряємо, чи тайл прохідний
                    var tileEntity = mapData.TileEntities[newX, newY];
                    if (tileEntity != null && tileEntity.IsAlive() && tileEntity.Has<MapTileComponent>())
                    {
                        ref var tile = ref tileEntity.Get<MapTileComponent>();
                        if (tile.TileType == 1)
                        {
                            playerObj.transform.position = newPosition;
                            positionComponent.x = newX;
                            positionComponent.y = newY;
                        }
                    }
                }
            }
        }
    }
}