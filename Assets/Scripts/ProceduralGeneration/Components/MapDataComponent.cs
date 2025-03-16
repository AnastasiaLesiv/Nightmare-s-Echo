using System;
using Leopotam.Ecs;
using Unity.VisualScripting;
using UnityEngine;

public struct MapDataComponent
{
    public int Width;
    public int Height;

    public int randomFillPercent;
    
    public EcsEntity[,] TileEntities;
    
}
