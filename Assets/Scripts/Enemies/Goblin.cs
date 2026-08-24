using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class Goblin : BaseEnemy
{
    [Header("Dash/Jump")]
    [SerializeField, Range(1, 20)] float attackRange;
    [SerializeField, Range(10, 40)] float dashForce;
    [SerializeField, Range(0, 20)] float jumpForce; // componente vertical do pulo em direção ao player

    [Header("Explosão")]
    [SerializeField, Range(0.1f, 5f)] float explosionDelay = 1f; // tempo até explodir após iniciar o dash
    [SerializeField, Range(1f, 10f)] float explosionRadius = 3f;
    [SerializeField] int explosionDamage;
    [SerializeField] LayerMask playerLayerMask;
    [SerializeField] ParticleSystem explosionPSystemPrefab;

    Rigidbody rb;
    CapsuleCollider myCollider;
    NavMeshAgent agent;
    Animator animator;
    [SerializeField]
    BomberState currentState;
    bool isAttacking;
    Coroutine attackRoutineRef;

    [SerializeField] AudioSource goblinJumping;


    enum BomberState
    {
        Hide,
        Idle,
        Walk,
        Attack,
        Die
    }

    protected override void Start()
    {
        base.Start();

        animator = GetComponent<Animator>();
        animator.speed = 0;
        life = maxLife;
        currentState = BomberState.Hide;
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        myCollider = GetComponent<CapsuleCollider>();
        Physics.IgnoreCollision(myCollider, player.GetComponent<CapsuleCollider>(), true);
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        switch (currentState)
        {
            case BomberState.Hide:
                if (myRoom == GameManager.room)
                {
                    StartCoroutine(Appear());
                }
                break;

            case BomberState.Idle:
                currentState = BomberState.Walk;
                break;

            case BomberState.Walk:
                float distToPlayer = Vector3.Distance(transform.position, GetPlayer().transform.position);
                animator.SetBool("Running", true);
                if (distToPlayer < attackRange)
                {
                    animator.SetBool("Running", false);
                    currentState = BomberState.Attack;
                }
                break;

            case BomberState.Attack:
                if (!isAttacking)
                {
                    attackRoutineRef = StartCoroutine(AttackRoutine());
                }
                break;

            case BomberState.Die:
                break;
        }
    }

    void FixedUpdate()
    {
        if (currentState == BomberState.Walk)
        {
            Vector3 dir = GetPlayer().transform.position - transform.position;
            dir.y = 0;
            if (dir != Vector3.zero)
            {
                rb.MoveRotation(Quaternion.LookRotation(dir));
                agent.SetDestination(GetPlayer().transform.position);
            }
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        myCollider.isTrigger = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.updatePosition = false;
        agent.updateRotation = false;

        animator.SetBool("Attacking", true);

        Vector3 dashDir = GetPlayer().transform.position - transform.position;
        dashDir.y = 0;
        dashDir = dashDir.normalized;
        rb.MoveRotation(Quaternion.LookRotation(dashDir));

        // ativa física real e dá o pulo/dash em direção ao player
        rb.isKinematic = false;
        rb.AddForce(dashDir * dashForce + Vector3.up * jumpForce, ForceMode.Impulse);

        goblinJumping.Play();

        // espera o tempo de explosão, independente de ter aterrissado ou não
        yield return new WaitForSeconds(explosionDelay);

        Explode();
    }

    void Explode()
    {
        currentState = BomberState.Die;

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, playerLayerMask);
        foreach (Collider col in hits)
        {
            if (col.CompareTag("Player"))
            {
                col.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth);
                playerHealth?.TakeDamage(explosionDamage);
            }
        }

        if (explosionPSystemPrefab != null)
        {
            ParticleSystem fx = Instantiate(explosionPSystemPrefab, transform.position, Quaternion.identity);
            fx.Play();
            Destroy(fx.gameObject, fx.main.duration);
        }

        //Destroy(gameObject);
        Die();
    }

    IEnumerator Appear()
    {
        yield return new WaitForSeconds(1f);
        animator.speed = 1;

        yield return new WaitForSeconds(1f);
        if (currentState == BomberState.Hide)
            currentState = BomberState.Idle;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}