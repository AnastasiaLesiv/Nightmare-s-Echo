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
        ref var mapData = ref mapEntity.Get<MapDataConponent>();
        mapData.Width = 120;
        mapData.Height = 80;
        mapData.randomFillPercent = 44;
        
        _tilemap = GameObject.Find("World").GetComponent<Tilemap>();

        GenerateMap(mapData);
    }

    void GenerateMap(MapDataConponent mapData)
    {
        map = new int[mapData.Width, mapData.Height];

        RandomFillMap(mapData.randomFillPercent);

        for (int x = 0; x < 10; x++)
        {
            SmoothMap();
        }

        for (int x = 0; x < mapData.Width; x++)
        {
            for (int y = 0; y < mapData.Height; y++)
            {
                int tileType = map[x, y];
                Vector3Int position = new Vector3Int(x, y, 0); // Координати для Tilemap

                //if (tileType != 0) // Пропускаємо порожні тайли
                
                TileBase tile = GetTileFromType(tileType); // Отримуємо відповідний Tile
                _tilemap.SetTile(position, tile); // Встановлюємо тайл у Tilemap
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

    TileBase GetTileFromType(int type)
    {
       // TileBase[] tiles = Resources.Load("Tiles");
        //foreach (var tile in tiles)
        //{
            if (type == 1)
            {
              
                    //Debug.Log("Sprite was found.");
                    return Resources.Load<TileBase>("Tiles/tile_1");
                
            }
            else if (type == 0)
            {
                 //Debug.Log("Sprite was found.");
                    return  Resources.Load<TileBase>("Tiles/tile_0");
                
            }
           
        return null;
    }
    
}
























