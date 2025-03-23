using Leopotam.Ecs;
using UnityEngine;

namespace Characters.Systems
{
    public class AnimationSystem : IEcsRunSystem
    {
        private readonly EcsFilter<PlayerTag, AnimationComponent, DirectionComponent> _animationFilter = null;
        private readonly EcsFilter<GameStateComponent> _gameStateFilter = null;

        public void Run()
        {
            if (_gameStateFilter.IsEmpty()) return;
            ref var gameState = ref _gameStateFilter.Get1(0);
            
            if (_animationFilter.IsEmpty())
            {
                Debug.LogWarning("AnimationSystem: No entities found with required components!");
                return;
            }

            Debug.Log($"AnimationSystem: Processing {_animationFilter.GetEntitiesCount()} entities");

            foreach (var i in _animationFilter)
            {
                ref var animationComponent = ref _animationFilter.Get2(i);
                ref var directionComponent = ref _animationFilter.Get3(i);

                if (animationComponent.Animator == null)
                {
                    Debug.LogError("AnimationSystem: Animator is null!");
                    continue;
                }

                Vector2 currentDirection = directionComponent.Direction;
                Debug.Log($"AnimationSystem: Current direction: {currentDirection}");

                // Оновлюємо стан руху
                bool isMoving = currentDirection != Vector2.zero;
                animationComponent.IsMoving = isMoving;
                Debug.Log($"AnimationSystem: Is moving: {isMoving}");

                // Оновлюємо швидкість для анімації
                animationComponent.MoveSpeed = isMoving && gameState.CurrentMode == GameMode.Exploration ? 1f : 0f;
                animationComponent.Animator.SetFloat("Speed", animationComponent.MoveSpeed);
                Debug.Log($"AnimationSystem: Setting Speed parameter to {animationComponent.MoveSpeed}");

                // Оновлюємо напрямок для анімації
                if (isMoving)
                {
                    animationComponent.LastDirection = currentDirection;
                    UpdateAnimationDirection(animationComponent.Animator, currentDirection);
                    Debug.Log($"AnimationSystem: Moving Direction: {currentDirection}");
                }
                else
                {
                    // Використовуємо останній напрямок руху для анімації спокою
                    UpdateAnimationDirection(animationComponent.Animator, animationComponent.LastDirection);
                    Debug.Log($"AnimationSystem: Idle Direction: {animationComponent.LastDirection}");
                }
            }
        }

        private void UpdateAnimationDirection(Animator animator, Vector2 direction)
        {
            // Нормалізуємо напрямок для кращої точності
            direction = direction.normalized;

            // Встановлюємо параметри напрямку для аніматора
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);

            Debug.Log($"AnimationSystem: Setting direction parameters - Horizontal: {direction.x}, Vertical: {direction.y}");
        }
    }
}