using UnityEngine;

public abstract class EntityMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public abstract void DisableMovement();
    public abstract void EnableMovement();
}
