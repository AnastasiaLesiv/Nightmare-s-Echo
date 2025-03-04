using UnityEngine;

public struct MapTileComponent
{
    public int X;
    public int Y;
    // 0 - порожнеча, 1 - зелена підлога, 2 - фіолетова підлога, 3 - синя підлога,
    // 4 - стіни, 5 - деталі (трава, вода), 6 - об'єкти (артефакти)
    public int TileType;
    // 0 - базовий (центр), 1 - лівий край, 2 - верхній край, 3 - лівий верхній край
    public int Variant;
}
