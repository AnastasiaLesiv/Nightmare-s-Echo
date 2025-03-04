using Leopotam.Ecs;
using UnityEngine;

public struct MapDataConponent
{
    public int Width;
    public int Height;

    public int randomFillPercent;
    
    public EcsEntity[,] TileEntities;
}
