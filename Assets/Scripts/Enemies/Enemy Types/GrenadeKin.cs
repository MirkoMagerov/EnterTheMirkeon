using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeKin : Enemy
{
    public float explosionRadius = 2f;
    public float explosionDamage = 1;
    public float fuseTime = 4f; // Tiempo antes de explotar automáticamente

    private bool isExploding = false;

    protected override void Start()
    {
        base.Start();
        Invoke(nameof(Explode), fuseTime);
    }

    private void Update()
    {
        if (!isExploding)
        {
            Move();
        }
    }

    public override void Move()
    {
        // Corre hacia el jugador
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void Explode()
    {
        if (isExploding) return;

        isExploding = true;
        // Aquí puedes añadir efectos visuales de explosión

        // Detectar jugadores dentro del radio de explosión
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerLife>().TakeDamage(explosionDamage);
            }
        }

        // Destruir el enemigo después de explotar
        Die();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Explode();
        }
    }

    public override void Die()
    {
        // Cancelar la invocación si muere antes de explotar
        CancelInvoke();
        base.Die();
    }
}
