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
        _systems.Run();
    }

    private void AddSystems()
    {
        
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
