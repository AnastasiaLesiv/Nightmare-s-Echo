using Leopotam.Ecs;
using UnityEngine;

public class GameManagerSystem : IEcsInitSystem, IEcsRunSystem
{
    private readonly EcsWorld _world = null;
    private readonly EcsFilter<GameStateComponent> _gameStateFilter = null;
    private readonly EcsFilter<PlayerTag, PositionComponent> _playerFilter = null;
    private readonly EcsFilter<MageTag, PositionComponent> _enemyFilter = null;
    
    public void Init()
    {
        var gameStateEntity = _world.NewEntity();
        ref var gameState = ref gameStateEntity.Get<GameStateComponent>();
        gameState.CurrentMode = GameMode.Initialization;
        Debug.Log($"GameManagerSystem - Initialized with mode: {gameState.CurrentMode}");
    }

    public void Run()
    {
        if (_gameStateFilter.IsEmpty())
        {
            Debug.Log($"GameStateComponent not found in GameManagerSystem");
            return;
        }

        foreach (var i in _gameStateFilter)
        {
            ref var gameState = ref _gameStateFilter.Get1(i);

            switch (gameState.CurrentMode)
            {
                case GameMode.Initialization:
                    if (IsInitializationComplete())
                    {
                        gameState.CurrentMode = GameMode.Exploration;
                        Debug.Log($"GameManagerSystem - Transitioned to mode: {gameState.CurrentMode}");
                    }
                    break;
                case GameMode.Exploration:
                    if (ShouldEnterCombat())
                    {
                        gameState.CurrentMode = GameMode.Combat;
                        Debug.Log($"GameManagerSystem - Transitioned to mode: {gameState.CurrentMode}");
                    }
                    // Перевіряємо, чи потрібно зберегти гру
                    else if (ShouldSaveGame())
                    {
                        gameState.CurrentMode = GameMode.Saving;
                        Debug.Log($"GameManagerSystem - Transitioned to mode: {gameState.CurrentMode}");
                    }
                    break;
                case GameMode.Combat:
                    // Перевіряємо, чи бій завершений
                    if (IsCombatFinished())
                    {
                        gameState.CurrentMode = GameMode.Exploration;
                        Debug.Log($"GameManagerSystem - Transitioned to mode: {gameState.CurrentMode}");
                    }
                    break;
                case GameMode.Saving:
                    // Після збереження повертаємося до дослідження
                    if (IsSavingComplete())
                    {
                        gameState.CurrentMode = GameMode.Exploration;
                        Debug.Log($"GameManagerSystem - Transitioned to mode: {gameState.CurrentMode}");
                    }
                    break;
            }
        }
    }
    
    private bool IsInitializationComplete()
    {
        // Перевіряємо, чи всі необхідні системи завершили ініціалізацію
        bool isMapInitialized = !_world.GetFilter(typeof(EcsFilter<MapDataComponent>)).IsEmpty();
        bool isPlayerInitialized = !_playerFilter.IsEmpty();
        bool areEnemiesInitialized = !_enemyFilter.IsEmpty();

        return isMapInitialized && isPlayerInitialized && areEnemiesInitialized;
        
    }
    
    private bool ShouldEnterCombat()
    {
        // Наприклад, якщо гравець поруч із ворогом
       
        foreach (var p in _playerFilter)
        {
            ref var playerPosition = ref _playerFilter.Get2(p);
            foreach (var e in _enemyFilter)
            {
                ref var enemyPosition = ref _enemyFilter.Get2(e);
                float distance = Vector2.Distance(
                    new Vector2(playerPosition.x, playerPosition.y),
                    new Vector2(enemyPosition.x, enemyPosition.y)
                );
                if (distance <= 1f) // Якщо ворог на сусідньому тайлі
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    private bool IsCombatFinished()
    {
        if (_enemyFilter.IsEmpty())
        {
            return true;
        }

        // Або якщо ворогів немає поруч із гравцем
        foreach (var p in _playerFilter)
        {
            ref var playerPosition = ref _playerFilter.Get2(p);
            foreach (var e in _enemyFilter)
            {
                ref var enemyPosition = ref _enemyFilter.Get2(e);
                float distance = Vector2.Distance(
                    new Vector2(playerPosition.x, playerPosition.y),
                    new Vector2(enemyPosition.x, enemyPosition.y)
                );
                if (distance <= 1f)
                {
                    return false;
                }
            }
        }
        return true;
    } // Ворогів поруч немає, бій завершений
    
    private bool ShouldSaveGame()
    {
        // Наприклад, якщо гравець натиснув клавішу для збереження (наприклад, "S")
        return Input.GetKeyDown(KeyCode.S);
    }

    // Перевірка, чи збереження завершено
    private bool IsSavingComplete()
    {
        // Тут буде логіка збереження гри (наприклад, запис у файл)
        // Для прикладу просто повертаємо true через 1 секунду
        return true; // Тимчасово
    }
    
}
