using Leopotam.Ecs;
using UnityEngine;
using Voody.UniLeo;

public class TileMono : MonoBehaviour, IConvertToEntity
{
    [SerializeField] internal Sprite sprite;
    [SerializeField] internal int tileType = 0;
    [SerializeField] internal int variant = 0;
    [SerializeField] internal int x;
    [SerializeField] internal int y;
    
    public void Convert(EcsEntity entity)
    {
        ref var tile = ref entity.Get<MapTileComponent>();
        tile.X = x;
        tile.Y = y;
        tile.TileType = tileType;
        //tile.Variant = variant;
        
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        
        if(sr == null) sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        //Debug.Log($"Setting sprite: {sprite?.name ?? "null"} for tile at ({x}, {y}) with type {tileType}");
    }
}
