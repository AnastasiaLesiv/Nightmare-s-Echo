using System;
using System.Globalization;
using Leopotam.Ecs;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;
using Random = System.Random;

public class LevelGenerationSystem : IEcsInitSystem
{
    private EcsWorld _world;
    private Tilemap _tilemap;
    private int[,] map;

    public void Init()
    {
        // створюємо сутність мапи, щоб зберігати множину компонентів
        var mapEntity = _world.NewEntity();
        ref var mapData = ref mapEntity.Get<MapDataComponent>();
        mapData.Width = 50;
        mapData.Height = 30;
        mapData.randomFillPercent = 44;
        mapData.TileEntities = new EcsEntity[mapData.Width, mapData.Height];
        
        _tilemap = GameObject.Find("World").GetComponent<Tilemap>();

        GenerateMap(ref mapData);
    }

    void GenerateMap(ref MapDataComponent mapData)
    {
        map = new int[mapData.Width, mapData.Height];

        RandomFillMap(mapData.randomFillPercent);

        for (int x = 0; x < 10; x++)
        {
            SmoothMap();
        }

        // 1 - purple, 2 - green, 3 - blue
        int color = 1;//RandomNumber(0, 3);

        for (int x = 0; x < mapData.Width; x++)
        {
            for (int y = 0; y < mapData.Height; y++)
            {
                int tileType = map[x, y];
                Vector3Int position = new Vector3Int(x, y, 0); // Координати для Tilemap
                
                var tileEntity = _world.NewEntity();
                ref var tileComponent = ref tileEntity.Get<MapTileComponent>();
                tileComponent.TileType = tileType;
                tileComponent.Color = color;
                tileComponent.Item = default;
                
                mapData.TileEntities[x, y] = tileEntity;
                
                

                //if (tileType != 0) // Пропускаємо порожні тайли
                
                TileBase tile = GetTileFromType(tileType, color); // Отримуємо відповідний Tile
                _tilemap.SetTile(position, tile); // Встановлюємо тайл у Tilemap
                //Debug.Log($"Tile spawned at {x},{y}. TileType: {tileType}");
            }
        }
    }

    void RandomFillMap(int randomFillPercent)
    {
        string seed = DateTime.Now.Ticks.ToString(); // Випадковий seed
        Random pseudoRandom = new Random(seed.GetHashCode());

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (x == 0 || x == map.GetLength(0) - 1 || y == 0 || y == map.GetLength(1) - 1)
                {
                    map[x, y] = 0;
                }
                
                map[x, y] = (pseudoRandom.Next(0, 100) > randomFillPercent) ? 1 : 0;
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;
                
                if (x == 0 || x == map.GetLength(0) - 1 || y == 0 || y == map.GetLength(1) - 1)
                {
                    map[x, y] = 0;
                }
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < map.GetLength(0) && neighbourY >= 0 &&
                    neighbourY < map.GetLength(1))
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
                
            }
        }
        return wallCount;
    } 

    TileBase GetTileFromType(int type, int color)
    {

            if (type == 1)
            {
                switch (color)
                {
                  case 1:   return Resources.Load<TileBase>("Tiles/purple_floor");
                  case 2:   return Resources.Load<TileBase>("Tiles/green_floor");
                  case 3:   return Resources.Load<TileBase>("Tiles/blue_floor");
                  
                }
                
            }
            /*if (type == 0)
            {
                 //Debug.Log("Sprite was found.");
                    return  Resources.Load<TileBase>("Tiles/tile_1");
                
            }*/
            return null;
    }

    int RandomNumber(int min, int max)
    {
        Random rand  = new Random();
        return  rand.Next(min, max + 1);
    }

    private void AddRandomObjects()
    {
        int objectsCount = map.GetLength(0) * map.GetLength(1) / 20;
        
        string[] objects = new string[objectsCount];

        for (int i = 0; i < objectsCount; i++)
        {
            int x = RandomNumber(0, map.GetLength(0) - 1);
            int y =RandomNumber(0, map.GetLength(1) - 1);
            
            Vector3Int position = new Vector3Int(x, y, 0);
            TileBase tile = _tilemap.GetTile(position);

            if (tile != null)
            {
                
            } // Якщо на цій позиції є підлога
            
        }
    }
}
























