using System;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    protected float currentAttackInterval;
    public int currentAttackCount;

    protected override void Update()
    {
        base.Update();

        if (currentAttackInterval > 0)
        {
            currentAttackInterval -= Time.deltaTime;
            if (currentAttackInterval <= 0) Attack(currentAttackCount);
        }
    }

    public override bool CanAttack()
    {
        if (currentAttackCount > 0) return true;
        return base.CanAttack();
    }

    protected override bool Attack(int attackCount = 1)
    {
        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning(string.Format("No prefab for {0}", name));
            currentCooldown = data.baseStats.cooldown;
            return false;
        }
        if (!CanAttack()) return false;

        float spawnAngle = GetSpawningAngle();

        Projectile prefab = Instantiate(currentStats.projectilePrefab,
            owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle),
            Quaternion.Euler(0, 0, spawnAngle));

        prefab.weapon = this;
        prefab.owner = owner;

        if (currentCooldown <= 0)
        {
            currentCooldown += currentStats.cooldown;
        }
        attackCount--;

        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }
        return true;
    }

    protected virtual float GetSpawningAngle()
    {
        return Mathf.Atan2(movement.lastMovedVector.y, movement.lastMovedVector.x) * Mathf.Rad2Deg;
    }

    protected virtual Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        return Quaternion.Euler(0, 0, spawnAngle) * new Vector2(
            UnityEngine.Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            UnityEngine.Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
        );
    }
}
