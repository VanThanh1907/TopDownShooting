using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ShootingEnemy : EnemyController
{
    public GameObject bulletPrefab;
    public float fireRate = 1f;
    private float fireTimer;

    protected override void Update()
    {
        base.Update();

        fireTimer += Time.deltaTime;
        if (fireTimer >= 1f / fireRate)
        {
            Fire();
            fireTimer = 0f;
        }
    }

    void Fire()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<BulletController>().SetDirection(dir);
    }
}
