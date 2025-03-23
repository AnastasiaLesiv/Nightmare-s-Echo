using UnityEngine;

public struct AnimationComponent
{
    public Animator Animator;
    public float MoveSpeed;
    public Vector2 LastDirection; // Останній напрямок руху для визначення напрямку в стані спокою
    public bool IsMoving;
} 