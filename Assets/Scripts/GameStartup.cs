using Characters.Systems;
using UnityEngine;
using Leopotam.Ecs;
using Voody.UniLeo;

public class GameStartup : MonoBehaviour
{
    private EcsWorld _world;
    private EcsSystems _systems; 
    
    void Start()
    {
        _world = new EcsWorld();
        _systems = new EcsSystems(_world);

        _systems.ConvertScene();
        
        AddInjections();
        AddSystems();
        AddOneFrames();
        
        _systems.Init();
    }

    void Update()
    {
        if (_systems != null)
        {
            _systems.Run();
        }
    }

    private void AddSystems()
    {
        _systems
            .Add(new GameManagerSystem())
            .Add(new LevelGenerationSystem())
            .Add(new PlayerInitializationSystem())
            .Add(new CameraFollowSystem())
            .Add(new MageInitializationSystem())
            .Add(new PlayerInputSystem())
            .Add(new MovementSystem())
            .Add(new MageMovementSystem())
            .Add(new AnimationSystem())
            .Add(new PlayerAttackInputSystem()) // Додаємо систему введення для атаки
            .Add(new CombatSystem());;
    }
    
    private void AddOneFrames()
    {
        
    }
    
    private void AddInjections()
    {
        
    }

    void OnDestroy()
    {
        if (_systems == null) return;
        
        _systems.Destroy();
        _systems = null;
        _world.Destroy();
        _world = null;
    }
}
