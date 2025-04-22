using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float projectileSpeed;
    public float attackDamage;
    public float abilityRange;
    public LayerMask enemyLayer;

    private Vector2 startPosition;

    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();
    private void Start()
    {
        startPosition = transform.position;

    }

    
    private void Update()
    {
        float distanceTravelled = Vector2.Distance(startPosition, transform.position);
        if (distanceTravelled >= abilityRange)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Projectile] Hit object: {other.name} on layer {LayerMask.LayerToName(other.gameObject.layer)}");

        if (other.gameObject.tag == "Obstacle")
        {
            Destroy(gameObject);
        }
        if (((1 << other.gameObject.layer) & enemyLayer) != 0 && !hitEnemies.Contains(other))
        {
            hitEnemies.Add(other);
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamageFromAbilities(attackDamage);
            }
        }
    }

    public void Initialize(float damage, float speed, float range, LayerMask layer)
    {
        attackDamage = damage;
        projectileSpeed = speed;
        abilityRange = range;
        enemyLayer = layer;
    }
}
