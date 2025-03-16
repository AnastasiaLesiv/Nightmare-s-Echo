using JetBrains.Annotations;
using Leopotam.Ecs;
using UnityEngine;
using Voody.UniLeo;

namespace Characters.Systems
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerMono : MonoBehaviour, IConvertToEntity
    {
        public Sprite sprite;
        [SerializeField] private float speed;
        [SerializeField] private int startX;
        [SerializeField] private int startY;

  
        public void Convert(EcsEntity entity)
        {
            ref var playerTag = ref entity.Get<PlayerTag>(); 
            playerTag.GameObject = gameObject;
       
            entity.Get<PlayerTag>();

            ref var position = ref entity.Get<PositionComponent>();
            position.x = startX;
            position.y = startY;
            position.TargetPosition = new Vector3(startX, startY, 0); // Ініціалізація цільової позиції
            position.IsMoving = false; // Ініціалізація стану
            Debug.Log($"Initialized PositionComponent at ({startX}, {startY})");

            ref var direction = ref entity.Get<DirectionComponent>();
            direction.Direction = Vector2.zero;
            Debug.Log("Initialized DirectionComponent");
       
            ref var moveSpeed = ref entity.Get<MovableComponent>();
            moveSpeed.Speed = speed;
            Debug.Log($"Initialized MovableComponent with Speed: {speed}");

            ref var stats = ref entity.Get<CharacterStatsComponent>();
            stats.health = 100;
            stats.attackPower = 10f;
            stats.defense = 5f;
            stats.dodgeChance = 0.2f;
      
        }

        public void SetPlayerData(int x, int y, float speed, Sprite sprite)
        {
            startX = x;
            startY = y;
            this.speed = speed;
            this.sprite = sprite;

            transform.position = new Vector3(x, y, 0);
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = sprite;
            }
        }
    }
}
