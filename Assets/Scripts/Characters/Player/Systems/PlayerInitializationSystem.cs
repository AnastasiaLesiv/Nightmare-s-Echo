using Leopotam.Ecs;
using UnityEngine;

namespace Characters.Systems
{
    public class PlayerInitializationSystem : IEcsInitSystem
    {
        private EcsWorld _world;
        private EcsFilter<MapDataComponent> _mapFilter;

        public void Init()
        {
            foreach (var i in _mapFilter)
            {
                ref var mapData = ref _mapFilter.Get1(i);
            
                int playerX = -1, playerY = -1;
                for (int attempt = 0; attempt < 100; attempt++)
                {
                    int x = Random.Range(1, mapData.Width - 1);
                    int y = Random.Range(1, mapData.Height - 1);

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

                        ref var position = ref playerEntity.Get<PositionComponent>();
                        position.x = playerX;
                        position.y = playerY;

                        ref var direction = ref playerEntity.Get<DirectionComponent>();
                        direction.Direction = Vector2.zero;

                        ref var movable = ref playerEntity.Get<MovableComponent>();
                        movable.Speed = 5f;

                        ref var animation = ref playerEntity.Get<AnimationComponent>();
                        animation.Animator = playerObj.GetComponent<Animator>();
                        animation.MoveSpeed = 0f;
                        animation.LastDirection = Vector2.down;
                        animation.IsMoving = false;
                        
                        ref var stats = ref playerEntity.Get<PlayerStatsComponent>();
                        stats.Health = 100;
                        stats.AttackPower = 10f;
                        stats.Defense = 5f;
                        stats.DodgeChance = 0.2f;
                    
                        PlayerMono playerMono = playerObj.GetComponent<PlayerMono>();
                        if (playerMono != null)
                        {
                            playerMono.SetPlayerData(playerX, playerY, 5f);
                        }
                    }
                }
            }
        }
    }
}
