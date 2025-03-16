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
        Debug.Log("EcsStartup Start - Systems initialized successfully");
    }

    void Update()
    {
        Debug.Log("EcsStartup Update - Running systems");
        if (_systems != null)
        {
            _systems.Run();
        }
        
    }

    private void AddSystems()
    {
        _systems
            .Add(new LevelGenerationSystem())
            .Add(new PlayerInitializationSystem())
            .Add(new PlayerInputSystem())
            .Add(new MovementSystem());
        Debug.Log("EcsStartup - Systems added successfully");
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
        
        Debug.Log("EcsStartup OnDestroy - Destroying systems");
       _systems.Destroy();
       _systems = null;
       _world.Destroy();
       _world = null;
    }
}
