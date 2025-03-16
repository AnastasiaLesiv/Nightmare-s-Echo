using UnityEngine;

public struct PositionComponent
{
    public int x;
    public int y;
    public Vector3 TargetPosition; // Цільова позиція для плавного руху
    public bool IsMoving; // Чи рухається персонаж
}
