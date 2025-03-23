using Leopotam.Ecs;
using UnityEngine;

namespace Characters.Systems
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
    public class MageMono : MonoBehaviour
    {
        //[SerializeField] private Sprite sprite;
        [SerializeField] private float speed = 3f;
        [SerializeField] private int startX;
        [SerializeField] private int startY;

        public void Convert(EcsEntity entity)
        {
            ref var enemyTag = ref entity.Get<MageTag>();
            enemyTag.GameObject = gameObject;

            ref var position = ref entity.Get<PositionComponent>();
            position.x = startX;
            position.y = startY;
            position.TargetPosition = new Vector3(startX, startY, 0);
            position.IsMoving = false;
            Debug.Log($"Initialized Enemy PositionComponent at ({startX}, {startY})");

            ref var direction = ref entity.Get<DirectionComponent>();
            direction.Direction = Vector2.zero;
            Debug.Log("Initialized Enemy DirectionComponent");

            ref var moveSpeed = ref entity.Get<MovableComponent>();
            moveSpeed.Speed = speed;
            Debug.Log($"Initialized Enemy MovableComponent with Speed: {speed}");

            ref var stats = ref entity.Get<EnemyStatsComponent>();
            stats.Health = 50f;
            stats.AttackPower = 5f;
            stats.Speed = speed;
        }

        public void SetEnemyData(int x, int y, float speed)
        {
            startX = x;
            startY = y;
            this.speed = speed;
            //this.sprite = sprite;

            transform.position = new Vector3(x, y, 0);
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();
            //sr.sprite = sprite;
            //sr.sortingLayerName = "Characters";
            //sr.sortingOrder = 10;

           /* if (sr.sprite == null)
            {
                Debug.LogError("Enemy sprite is null. Check Resources/Sprites or assign in Inspector.");
                sr.sprite = Resources.Load<Sprite>("Sprites/enemy_default");
            }*/
        }
    }
}