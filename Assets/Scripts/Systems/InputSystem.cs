using Leopotam.Ecs;
using UnityEngine;

public class InputSystem : IEcsRunSystem
{
    private readonly EcsFilter<PlayerTag, DirectionComponent> directionFilter = null;

    private float moveX;
    private float moveY;

    public void Run()
    {
        foreach (var i in directionFilter)
        {
            ref var directionComponent = ref directionFilter.Get2(i);
            ref var direction = ref directionComponent.Direction;
        }
    }

    private void SetDerection()
    {
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");
    }

}
