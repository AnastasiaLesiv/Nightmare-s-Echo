using Leopotam.Ecs;
using UnityEngine;

public class MageMovementSystem : IEcsRunSystem
{
    private readonly EcsWorld _world = null;
    private readonly EcsFilter<MageTag, PositionComponent, DirectionComponent, MovableComponent> _enemyFilter = null;
    private readonly EcsFilter<MapDataComponent> _mapFilter = null;
    private readonly EcsFilter<GameStateComponent> _gameStateFilter = null;

    private const float MoveSpeed = 3f;
    private const float MoveInterval = 1f; // Інтервал між рухами (секунди)

    private float _timer = 0f;
    
    public void Run()
    {
        if (_mapFilter.IsEmpty())
        {
            Debug.LogWarning("MapDataComponent not found in EnemyMovementSystem!");
            return;
        }

        if (_gameStateFilter.IsEmpty())
        {
            Debug.LogWarning("GameStateComponent not found in EnemyMovementSystem!");
            return;
        }
        
        ref var gameState = ref _gameStateFilter.Get1(0);
        if (gameState.CurrentMode != GameMode.Exploration)
        {
            return; // Рух можливий лише в режимі Exploration
        }
        
        _timer += Time.deltaTime;
        if (_timer < MoveInterval) return; // Чекаємо інтервал між рухами
        _timer = 0f;
        
        Debug.Log($"EnemyMovementSystem running - Filter count: {_enemyFilter.GetEntitiesCount()}");

        foreach (var i in _enemyFilter)
        {
            ref var enemyTag = ref _enemyFilter.Get1(i);
            ref var positionComponent = ref _enemyFilter.Get2(i);
            ref var directionComponent = ref _enemyFilter.Get3(i);
            ref var movableComponent = ref _enemyFilter.Get4(i);

            Vector2 direction = directionComponent.Direction;
            Debug.Log($"Enemy {i} - Current Position: ({positionComponent.x}, {positionComponent.y}), Direction: {direction}");

            if (!positionComponent.IsMoving)
            {
                // Випадковий напрямок
                direction = GetRandomDirection();
                directionComponent.Direction = direction;
                if (direction == Vector2.zero)
                {
                    Debug.Log("No movement for enemy");
                    continue;
                }

                int newX = positionComponent.x + (int)Mathf.Sign(direction.x);
                int newY = positionComponent.y + (int)Mathf.Sign(direction.y);
                Debug.Log($"Enemy {i} trying to move to ({newX}, {newY})");

                if (IsValidMove(newX, newY))
                {
                    Debug.Log($"Enemy {i} move to ({newX}, {newY}) is valid");
                    positionComponent.TargetPosition = new Vector3(newX, newY, 0);
                    positionComponent.IsMoving = true;
                }
                else
                {
                    Debug.LogWarning($"Enemy {i} move to ({newX}, {newY}) is invalid");
                    directionComponent.Direction = Vector2.zero;
                }
            }
            if (positionComponent.IsMoving)
            {
                GameObject enemyObj = enemyTag.GameObject;
                if (enemyObj != null)
                {
                    Animator animator = enemyObj.GetComponent<Animator>();
                    if (animator != null)
                    {
                        animator.SetFloat("MoveX", direction.x);
                        animator.SetFloat("MoveY", direction.y);
                        animator.SetBool("IsMoving", positionComponent.IsMoving);
                    }

                    Vector3 currentPosition = enemyObj.transform.position;
                    enemyObj.transform.position = Vector3.MoveTowards(currentPosition, positionComponent.TargetPosition, MoveSpeed * Time.deltaTime);
                    
                    if (Vector3.Distance(enemyObj.transform.position, positionComponent.TargetPosition) < 0.01f)
                    {
                        enemyObj.transform.position = positionComponent.TargetPosition;
                        positionComponent.x = (int)positionComponent.TargetPosition.x;
                        positionComponent.y = (int)positionComponent.TargetPosition.y;
                        positionComponent.IsMoving = false;
                        directionComponent.Direction = Vector2.zero;
                        Debug.Log($"Enemy {i} reached target position: ({positionComponent.x}, {positionComponent.y})");
                    }
                }
                else
                {
                    Debug.LogError($"Enemy GameObject is null for entity {i}");
                }
            }
        }
    }
    
    private Vector2 GetRandomDirection()
    {
        int random = Random.Range(0, 5);
        return random switch
        {
            0 => Vector2.up,
            1 => Vector2.down,
            2 => Vector2.left,
            3 => Vector2.right,
            _ => Vector2.zero
        };
    }
    
    private bool IsValidMove(int x, int y)
    {
        foreach (var i in _mapFilter)
        {
            ref var mapData = ref _mapFilter.Get1(i);
            if (x < 0 || x >= mapData.Width || y < 0 || y >= mapData.Height)
            {
                Debug.Log($"Invalid move for enemy - Out of bounds: ({x}, {y})");
                return false;
            }

            var tileEntity = mapData.TileEntities[x, y];
            if (tileEntity.IsAlive() && tileEntity.Has<MapTileComponent>())
            {
                ref var tile = ref tileEntity.Get<MapTileComponent>();
                bool isValid = tile.TileType == 1;
                Debug.Log($"Tile at ({x}, {y}) - Type: {tile.TileType}, Valid: {isValid}");
                return isValid;
            }
            else
            {
                Debug.LogWarning($"No TileComponent or entity not alive at ({x}, {y})");
                return false;
            }
        }
        return false;
    }
}
