using Leopotam.Ecs;
using UnityEngine;

namespace Characters.Systems
{
    public class PlayerInputSystem : IEcsRunSystem
    {
        private readonly EcsFilter<PlayerTag, DirectionComponent> directionFilter = null;
        public void Run()
        {
           // Debug.Log("Player Input System initiated");
        
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            // Пріоритет горизонтального руху над вертикальним
            Vector2 inputDirection = Vector2.zero;
            if (moveX != 0)
            {
                inputDirection = new Vector2(moveX, 0);
            }
            else if (moveY != 0)
            {
                inputDirection = new Vector2(0, moveY);
            }

            //Debug.Log($"Input - Horizontal: {moveX}, Vertical: {moveY}, Normalized Direction: {inputDirection}");

            foreach (var i in directionFilter)
            {
                ref var directionComponent = ref directionFilter.Get2(i);
                ref var positionComponent = ref directionFilter.GetEntity(i).Get<PositionComponent>(); // Додаємо доступ до PositionComponent

                if (!positionComponent.IsMoving) // Оновлюємо напрямок тільки якщо не рухаємося
                {
                    directionComponent.Direction = inputDirection;
                    //Debug.Log($"Updated Direction for entity {i} to {directionComponent.Direction}");
                }
            }
        }
    }
}
