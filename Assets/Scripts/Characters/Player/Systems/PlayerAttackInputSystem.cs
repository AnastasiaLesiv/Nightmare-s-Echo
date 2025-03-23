using Characters.GeneralComponents;
using Leopotam.Ecs;
using UnityEngine;

namespace Characters.Systems
{
    public class PlayerAttackInputSystem : IEcsRunSystem
    {
        private readonly EcsWorld _world = null;
        private readonly EcsFilter<PlayerTag, PositionComponent> _playerFilter = null;
        private readonly EcsFilter<MageTag, PositionComponent> _enemyFilter = null;
        private readonly EcsFilter<GameStateComponent> _gameStateFilter = null;
        
        public void Run()
        {
            if (_gameStateFilter.IsEmpty()) return;

            ref var gameState = ref _gameStateFilter.Get1(0);
            if (gameState.CurrentMode != GameMode.Combat) return; // Атака можлива лише в режимі Combat

            if (!Input.GetKeyDown(KeyCode.Space)) return; // Атака на клавішу Space
            foreach (var p in _playerFilter)
            {
                ref var playerEntity = ref _playerFilter.GetEntity(p);
                ref var playerPosition = ref _playerFilter.Get2(p);

                // Шукаємо найближчого ворога
                EcsEntity nearestEnemy = default;
                float minDistance = float.MaxValue;

                foreach (var e in _enemyFilter)
                {
                    ref var enemyPosition = ref _enemyFilter.Get2(e);
                    float distance = Vector2.Distance(
                        new Vector2(playerPosition.x, playerPosition.y),
                        new Vector2(enemyPosition.x, enemyPosition.y)
                    );
                    if (distance <= 1f && distance < minDistance)
                    {
                        minDistance = distance;
                        nearestEnemy = _enemyFilter.GetEntity(e);
                    }

                    if (nearestEnemy.IsAlive())
                    {
                        // Додаємо запит на атаку
                        ref var attackRequest = ref playerEntity.Get<AttackRequest>();
                        attackRequest.Target = nearestEnemy;
                        Debug.Log(
                            $"Player initiated attack on enemy at ({nearestEnemy.Get<PositionComponent>().x}, {nearestEnemy.Get<PositionComponent>().y})");
                    }
                }
            }
        }
    }
}