using Leopotam.Ecs;
using UnityEngine;
using System.Numerics;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;


public class MovementSystem : IEcsRunSystem
{
    private readonly EcsWorld _world = null;
    private readonly EcsFilter<MovableComponent, ModelComponent, DirectionComponent> movableFilter = null;
    
    
    public void Run()
    {
        foreach (var i in movableFilter)
        {
            ref var movableComponent = ref movableFilter.Get1(i);
            ref var modelComponent = ref movableFilter.Get2(i);
            ref var directionComponent = ref movableFilter.Get3(i);
            
            ref var direction = ref directionComponent.Direction;
            ref var transform = ref modelComponent.ModelTransform;
            float speed = movableComponent.Speed;
            
            Vector2 movement = direction * speed * Time.deltaTime;
            
            //transform.position = new Vector2(transform.position.x, transform.position.y) + movement;
        }
    }
    
}
