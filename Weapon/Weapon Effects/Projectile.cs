using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : WeaponEffect
{
    public enum DamageSource { projectile, owner };
    public DamageSource damageSource = DamageSource.projectile;
    [SerializeField]
    public bool hasAutoAim = false;
    public Vector3 rotationSpeed = new Vector3(0, 0, 0);

    protected Rigidbody2D rb;
    protected int piercing;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Weapon.Stats stats = weapon.GetStat();
        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.angularVelocity = rotationSpeed.z;
            rb.velocity = transform.right * stats.speed;
        }

        float area = stats.area == 0 ? 1 : stats.area;
        transform.localScale = new Vector3(
            area * Mathf.Sign(transform.localScale.x),
            area * Mathf.Sign(transform.localScale.y), 1
        );

        piercing = stats.piercing;

        if (stats.lifespan > 0)
        {
            Destroy(gameObject, stats.lifespan);
        }

        if (hasAutoAim) AccquireAutoAimFacing();
    }

    public virtual void AccquireAutoAimFacing()
    {
        float aimAngle;

        EnemyStat[] targets = FindObjectsOfType<EnemyStat>();

        if (targets.Length > 0)
        {
            EnemyStat selectedTarget = targets[Random.Range(0, targets.Length)];
            Vector2 difference = selectedTarget.transform.position - transform.position;
            aimAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        }
        else
        {
            aimAngle = Random.Range(0f, 360f);
        }

        transform.rotation = Quaternion.Euler(0, 0, aimAngle);
    }

    protected virtual void FixedUpdate()
    {
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            Weapon.Stats stats = weapon.GetStat();
            transform.position += transform.right * stats.speed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position);
            transform.Rotate(rotationSpeed * Time.fixedDeltaTime);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        EnemyStat es = other.GetComponent<EnemyStat>();
        BreakableProps breakableProps = other.GetComponent<BreakableProps>();

        if (es)
        {
            // Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;
            // es.TakeDamage(GetDamage(), source);
            es.TakeDamage(GetDamage());
            Weapon.Stats stats = weapon.GetStat();
            piercing--;
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }
        else if (breakableProps)
        {
            breakableProps.TakeDamage(GetDamage());
            piercing--;
            Weapon.Stats stats = weapon.GetStat();
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }
        if (piercing <= 0) { Destroy(gameObject); }
    }
}
