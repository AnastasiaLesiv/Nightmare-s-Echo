using Leopotam.Ecs;
using UnityEngine;

namespace Characters.Systems
{
    public class PlayerInitializationSystem : IEcsInitSystem
    {
        private EcsWorld _world;
        private EcsFilter<MapDataComponent> _mapFilter;
        
        public PlayerInitializationSystem()
        {
            Debug.Log("PlayerInitializationSystem - Constructor called");
        }

        public void Init()
        {
            Debug.Log("PlayerInitializationSystem - Init called");
            foreach (var i in _mapFilter)
            {
                Debug.Log("MapDataComponent found - Starting player initialization");
                ref var mapData = ref _mapFilter.Get1(i);
            
                int playerX = -1, playerY = -1;
                for (int attempt = 0; attempt < 100; attempt++)
                {
                    int x = Random.Range(1, mapData.Width - 1);
                    int y = Random.Range(1, mapData.Height - 1);
                    Debug.Log($"Checking position ({x}, {y})");
                    //Debug.Log(mapData.TileEntities[x, y].Get<MapTileComponent>().TileType);

                
                    if (mapData.TileEntities[x, y].Has<MapTileComponent>() &&
                        mapData.TileEntities[x, y].Get<MapTileComponent>().TileType == 1)
                    {
                        playerX = x;
                        playerY = y;
                        break;
                    }
                
                }

                if (playerX > -1 && playerY > -1)
                {
                    GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
                    if (playerPrefab != null)
                    {
                        GameObject playerObj = Object.Instantiate(playerPrefab, new Vector3(playerX, playerY, 0), Quaternion.identity);
                        EcsEntity playerEntity = _world.NewEntity();
                        
                        ref var playerTag = ref playerEntity.Get<PlayerTag>();
                        playerTag.GameObject = playerObj;
                        Debug.Log($"Added PlayerTag to entity {playerEntity}");

                        ref var position = ref playerEntity.Get<PositionComponent>();
                        position.x = playerX;
                        position.y = playerY;
                        Debug.Log($"Added PositionComponent to entity {playerEntity} - ({playerX}, {playerY})");

                        ref var direction = ref playerEntity.Get<DirectionComponent>();
                        direction.Direction = Vector2.zero;
                        Debug.Log($"Added DirectionComponent to entity {playerEntity}");

                        ref var movable = ref playerEntity.Get<MovableComponent>();
                        movable.Speed = 5f;
                        Debug.Log($"Added MovableComponent to entity {playerEntity}");
                    
                        PlayerMono playerMono = playerObj.GetComponent<PlayerMono>();
                        if (playerMono != null)
                        {
                            playerMono.SetPlayerData(playerX, playerY, 5f, playerMono.sprite); // Оновлюємо позицію
                            Debug.Log($"Player spawned at ({playerX}, {playerY}) using prefab");
                        }
                        else
                        {
                            Debug.LogError("PlayerMono component not found on prefab!");
                        }
                    }
                    else
                    {
                        Debug.LogError("Player prefab not found in Resources/Prefabs!");
                    }
                }
                else
                {
                    Debug.LogError("Could not find a valid spawn position for the player!");
                }
            
            }
        }
    }
}
