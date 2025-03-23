using Characters.GeneralComponents;
using Leopotam.Ecs;
using UnityEngine;

namespace Characters.Systems
{
    public class CombatSystem : IEcsRunSystem
    {
        private readonly EcsWorld _world = null;
        private readonly EcsFilter<GameStateComponent> _gameStateFilter = null;
        private readonly EcsFilter<PlayerTag, PlayerStatsComponent, AttackRequest, PositionComponent, PlayerStatsComponent> _playerAttackFilter = null;
        

        private readonly EcsFilter<MageTag, PositionComponent, EnemyStatsComponent> _enemyFilter = null;

        private float _enemyAttackTimer = 0f;
        private const float EnemyAttackInterval = 2f; // Вороги атакують кожні 2 секунди
        
        public void Run()
        {
            if (_gameStateFilter.IsEmpty()) return;

            ref var gameState = ref _gameStateFilter.Get1(0);
            if (gameState.CurrentMode != GameMode.Combat) return;
            
            foreach (var p in _playerAttackFilter)
            {
                ref var playerStats = ref _playerAttackFilter.Get2(p);
                ref var attackRequest = ref _playerAttackFilter.Get3(p);

                if (attackRequest.Target.IsAlive() && attackRequest.Target.Has<EnemyStatsComponent>())
                {
                    ref var enemyStats = ref attackRequest.Target.Get<EnemyStatsComponent>();
                    float damage = CalculateDamage(playerStats.AttackPower, 0f, enemyStats.Health); // Поки що захист ворога = 0
                    enemyStats.Health -= damage;
                    Debug.Log($"Player attacked enemy! Damage: {damage}, Enemy Health: {enemyStats.Health}");

                    if (enemyStats.Health <= 0)
                    {
                        DestroyEnemy(attackRequest.Target);
                    }
                }

                // Видаляємо запит на атаку після обробки
                _playerAttackFilter.GetEntity(p).Del<AttackRequest>();
            }
            
            _enemyAttackTimer += Time.deltaTime;
            if (_enemyAttackTimer >= EnemyAttackInterval)
            {
                _enemyAttackTimer = 0f;
                AttackPlayer();
            }
        }
        
        private float CalculateDamage(float attackPower, float defense, float targetHealth)
        {
            float damage = Mathf.Max(attackPower - defense, 1f); // Мінімальна шкода = 1
            if (Random.value < 0.2f) // 20% шанс ухилення (тимчасово)
            {
                Debug.Log("Target dodged the attack!");
                return 0f;
            }
            return damage;
        }

        private void DestroyEnemy(EcsEntity enemyEntity)
        {
            if (enemyEntity.Has<MageTag>())
            {
                ref var enemyTag = ref enemyEntity.Get<MageTag>();
                if (enemyTag.GameObject != null)
                {
                    Object.Destroy(enemyTag.GameObject);
                }
            }
            enemyEntity.Destroy();
            Debug.Log("Enemy destroyed!");
        }

        private void AttackPlayer()
        {
            foreach (var p in _playerAttackFilter)
            {
                ref var playerPosition = ref _playerAttackFilter.Get4(p);
                ref var playerStats = ref _playerAttackFilter.Get5(p);

                foreach (var e in _enemyFilter)
                {
                    ref var enemyPosition = ref _enemyFilter.Get2(e);
                    ref var enemyStats = ref _enemyFilter.Get3(e);

                    float distance = Vector2.Distance(
                        new Vector2(playerPosition.x, playerPosition.y),
                        new Vector2(enemyPosition.x, enemyPosition.y)
                    );
                    if (distance <= 1f)
                    {
                        float damage = CalculateDamage(enemyStats.AttackPower, playerStats.Defense, playerStats.Health);
                        playerStats.Health -= (int)damage;
                        Debug.Log($"Enemy attacked player! Damage: {damage}, Player Health: {playerStats.Health}");

                        if (playerStats.Health <= 0)
                        {
                            Debug.Log("Player defeated! Game Over!");
                            // Тут можна додати логіку завершення гри
                            break;
                        }
                    }
                }
            }
        }
    }
}