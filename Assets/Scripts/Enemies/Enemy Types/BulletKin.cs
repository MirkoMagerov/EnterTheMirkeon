using UnityEngine;

public class BulletKin : BaseEnemy
{
    [Header("BulletKin Movement")]
    public float wanderRadius = 1f;
    public float changeDirectionInterval = 0.5f;
    public float shootInterval = 1f;
    public float shootingRange = 5f;

    [Header("BulletKin Combat")]
    public float bulletSpeed = 5f;
    public int damageAmount = 1;

    private float lastDirectionChangeTime;
    private float lastShootTime;
    private Vector2 currentMovementDirection;

    private BulletPoolManager bulletPool;
    private EnemyStateMachine stateMachine;

    public enum BulletKinState
    {
        Idle,
        Wandering,
        Shooting
    }

    private void Awake()
    {
        // Configuraciones iniciales
        stateMachine = new EnemyStateMachine();
        bulletPool = FindObjectOfType<BulletPoolManager>();

        // Configuraciones por defecto
        moveSpeed = 2f;
        maxHealth = 10f;
    }

    private void Start()
    {
        // Estado inicial
        stateMachine.ChangeState(BulletKinState.Idle);
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (playerTransform == null) return;

        switch (stateMachine.CurrentState)
        {
            case BulletKinState.Idle:
                HandleIdleState();
                break;
            case BulletKinState.Wandering:
                HandleWanderingState();
                break;
            case BulletKinState.Shooting:
                HandleShootingState();
                break;
        }
    }

    private void HandleIdleState()
    {
        // Transición a wandering si el jugador está cerca
        if (Vector2.Distance(transform.position, playerTransform.position) <= shootingRange)
        {
            stateMachine.ChangeState(BulletKinState.Wandering);
        }
    }

    private void HandleWanderingState()
    {
        // Cambiar dirección periódicamente
        if (Time.time - lastDirectionChangeTime > changeDirectionInterval)
        {
            ChangeMovementDirection();
            lastDirectionChangeTime = Time.time;
        }

        // Movimiento aleatorio
        Vector2 movement = currentMovementDirection * moveSpeed * Time.deltaTime;
        transform.Translate(movement);

        // Lógica de disparo
        //HandleShooting();

        // Opcional: Volver a idle si el jugador está muy lejos
        if (Vector2.Distance(transform.position, playerTransform.position) > shootingRange * 1.5f)
        {
            stateMachine.ChangeState(BulletKinState.Idle);
        }
    }

    private void HandleShootingState()
    {
        // Disparar si el jugador está en rango
        if (Time.time - lastShootTime > shootInterval)
        {
            if (IsPlayerInRange())
            {
                Shoot();
                lastShootTime = Time.time;
            }
        }

        // Volver a wandering si está en rango
        stateMachine.ChangeState(BulletKinState.Wandering);
    }

    private void ChangeMovementDirection()
    {
        // Genera una dirección aleatoria dentro de un radio
        currentMovementDirection = Random.insideUnitCircle.normalized * wanderRadius;
    }

    private bool IsPlayerInRange()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        return distanceToPlayer <= shootingRange;
    }

    private void Shoot()
    {
        Vector2 shootDirection = (playerTransform.position - transform.position).normalized;
        //bulletPool.ShootBullet(transform.position, shootDirection, bulletSpeed, damageAmount);
    }

    public override void Initialize(Transform player)
    {
        base.Initialize(player);
        stateMachine.ChangeState(BulletKinState.Idle);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        // Opcional: Añadir efectos visuales o sonido de daño
        // Ejemplo: PlayHitSound();
    }

    public override void Die()
    {
        // Lógica específica de muerte para BulletKin
        // Ejemplo: Spawn pickups, play death animation
        base.Die();
    }

    // Método opcional para personalizar el comportamiento
    public void SetDifficultyParameters(float healthMultiplier, float speedMultiplier)
    {
        maxHealth *= healthMultiplier;
        moveSpeed *= speedMultiplier;
        currentHealth = maxHealth;
    }

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateBehavior()
    {
        throw new System.NotImplementedException();
    }
}