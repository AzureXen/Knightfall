using UnityEngine;

public class DeflectedBulletScript : EnemyBullet
{
    public override void SetDirection(Vector2 dir)
    {
        direction = dir;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
