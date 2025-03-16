using Leopotam.Ecs;
using UnityEngine;

public struct MapTileComponent
{
    public int X, Y;
    public int TileType; // 0 - порожнеча, 1 - підлога, 2 - стіна
    public int Color; // 1 - purple, 2 - green, 3 - blue
    public EcsEntity Item; // Посилання на предмет (null, якщо немає предмета) 0 - базовий (центр), 1 - лівий край, 2 - верхній край, 3 - лівий верхній край
    public int Variant;
}
