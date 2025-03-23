using Characters.Systems;
using Leopotam.Ecs;
using UnityEngine;

public class MageInitializationSystem : IEcsInitSystem
{
    private EcsWorld _world;
    private EcsFilter<MapDataComponent> _mapFilter;
    private EcsFilter<PlayerTag, PositionComponent> _playerFilter;

    private const int EnemyCount = 3;
    private const float MinDistanceFromPlayer = 5f;

    public void Init()
    { 
        Vector2? playerPosition = null;
        foreach (var playerIdx in _playerFilter)
        {
            ref var playerPos = ref _playerFilter.Get2(playerIdx);
            playerPosition = new Vector2(playerPos.x, playerPos.y);
            break;
        }

        foreach (var i in _mapFilter)
        {
            ref var mapData = ref _mapFilter.Get1(i);

            for (int mageIndex = 0; mageIndex < EnemyCount; mageIndex++)
            {
                int mageX = -1, mageY = -1;
                bool validPositionFound = false;

                for (int attempt = 0; attempt < 100 && !validPositionFound; attempt++)
                {
                    int x = Random.Range(1, mapData.Width - 1);
                    int y = Random.Range(1, mapData.Height - 1);
                    
                    if (playerPosition.HasValue)
                    {
                        float distanceToPlayer = Vector2.Distance(
                            new Vector2(x, y),
                            playerPosition.Value
                        );
                        
                        if (distanceToPlayer < MinDistanceFromPlayer)
                        {
                            continue;
                        }
                    }

                    if (IsValidPosition(x, y, mapData))
                    {
                        mageX = x;
                        mageY = y;
                        validPositionFound = true;
                        break;
                    }
                }

                if (validPositionFound)
                {
                    SpawnMage(mageIndex, mageX, mageY);
                }
            }
        }
    }

    private bool IsValidPosition(int x, int y, MapDataComponent mapData)
    {
        if (x < 0 || x >= mapData.Width || y < 0 || y >= mapData.Height)
        {
            return false;
        }

        var tileEntity = mapData.TileEntities[x, y];
        if (!tileEntity.IsAlive() || !tileEntity.Has<MapTileComponent>())
        {
            return false;
        }

        ref var tile = ref tileEntity.Get<MapTileComponent>();
        return tile.TileType == 1;
    }

    private void SpawnMage(int mageIndex, int x, int y)
    {
        GameObject enemyPrefab = Resources.Load<GameObject>("Prefabs/Mage");
        if (enemyPrefab == null) return;

        GameObject enemyObj = Object.Instantiate(enemyPrefab, new Vector3(x, y, 0), Quaternion.identity);
        EcsEntity enemyEntity = _world.NewEntity();

        ref var enemyTag = ref enemyEntity.Get<MageTag>();
        enemyTag.GameObject = enemyObj;

        ref var position = ref enemyEntity.Get<PositionComponent>();
        position.x = x;
        position.y = y;
        position.TargetPosition = new Vector3(x, y, 0);
        position.IsMoving = false;
        
        ref var direction = ref enemyEntity.Get<DirectionComponent>();
        direction.Direction = Vector2.zero;

        ref var movable = ref enemyEntity.Get<MovableComponent>();
        movable.Speed = 3f;

        ref var stats = ref enemyEntity.Get<EnemyStatsComponent>();
        stats.Health = 50f;
        stats.AttackPower = 5f;
        stats.Speed = 3f;

        ref var animation = ref enemyEntity.Get<AnimationComponent>();
        animation.Animator = enemyObj.GetComponent<Animator>();
        animation.MoveSpeed = 0f;
        animation.LastDirection = Vector2.down;
        animation.IsMoving = false;

        MageMono enemyMono = enemyObj.GetComponent<MageMono>();
        if (enemyMono != null)
        {
            enemyMono.SetEnemyData(x, y, 3f);
        }
    }
}
