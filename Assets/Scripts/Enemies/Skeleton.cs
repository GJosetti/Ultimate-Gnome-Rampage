using System.Collections;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public class Skeleton : BaseEnemy
{
    [SerializeField] int damage;
    [SerializeField, Range(1, 20)] float attackRange;
    [SerializeField, Range(0, 2)] float attackStunTime = 0.5f;
    [SerializeField, Range(1, 20)] float attackForce;
    [SerializeField, Range(1, 20)] float attackCooldown;
    [SerializeField, Range(0.05f, 1f)] float attackDashDuration = 0.2f; // duração do "dash" em si

    [Header("Attack Hitbox")]
    [SerializeField] Vector3 attackBoxSize = new Vector3(1f, 1.5f, 1.5f);
    [SerializeField] float attackBoxForwardOffset = 1f;

    [SerializeField]
    float attackTimer = 0;
    Rigidbody rb;
    NavMeshAgent agent;
    Animator animator;
    [SerializeField]
    SkeletonState currentState;
    bool isAttacking;
    bool hasHitPlayerThisAttack;

    enum SkeletonState
    {
        Hide,
        Idle,
        Walk,
        Attack,
        Die
    }

    // centro da hitbox de ataque: um pouco à frente do inimigo, na altura do corpo
    Vector3 AttackBoxCenter => transform.position + transform.forward * attackBoxForwardOffset + Vector3.up * (attackBoxSize.y / 2f);

    protected override void Start()
    {
        base.Start();

        animator = GetComponent<Animator>();
        animator.speed = 0;
        maxLife = 10;
        life = maxLife;
        currentState = SkeletonState.Hide;
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }
    // Lógica
    void Update()
    {
        switch (currentState)
        {
            case SkeletonState.Hide:
                if (myRoom == GameManager.room)
                {
                    StartCoroutine(Appear());
                }
                break;
            case SkeletonState.Idle:
                if (attackTimer <= 0)
                {
                    currentState = SkeletonState.Walk;
                    break;
                }
                attackTimer -= Time.deltaTime;
                break;
            case SkeletonState.Walk:

                float distToPlayer = Vector3.Distance(transform.position, GetPlayer().transform.position);
                animator.SetBool("Running", true);
                if (distToPlayer < attackRange)
                {
                    animator.SetBool("Running", false);
                    currentState = SkeletonState.Attack;
                }

                break;
            case SkeletonState.Attack:
                if (!isAttacking)
                {
                    StartCoroutine(AttackRoutine());
                }
                break;
            case SkeletonState.Die:
                break;
        }
    }
    //Física
    void FixedUpdate()
    {
        switch (currentState)
        {
            case SkeletonState.Walk:
                Vector3 dir = GetPlayer().transform.position - transform.position;
                dir.y = 0;
                if (dir != Vector3.zero)
                {
                    rb.MoveRotation(Quaternion.LookRotation(dir));
                    agent.SetDestination(GetPlayer().transform.position);
                }
                break;
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        hasHitPlayerThisAttack = false;

        // desliga o NavMeshAgent enquanto controlamos o movimento via Rigidbody
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        animator.SetBool("Attacking", true);

        // durante a preparação, continua rotacionando pro player em vez de ficar travado
        float windupTimer = 0f;
        Vector3 dashDir = transform.forward;
        while (windupTimer < attackStunTime)
        {
            Vector3 lookDir = GetPlayer().transform.position - transform.position;
            lookDir.y = 0;

            if (lookDir != Vector3.zero)
            {
                dashDir = lookDir.normalized;
                rb.MoveRotation(Quaternion.LookRotation(dashDir));
            }

            windupTimer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        float elapsed = 0f;
        while (elapsed < attackDashDuration)
        {
            rb.MovePosition(rb.position + dashDir * attackForce * Time.fixedDeltaTime);
            CheckAttackHit();
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // para o dash
        rb.linearVelocity = Vector3.zero;

        // sincroniza o agent com a posição real após o movimento manual
        agent.Warp(transform.position);
        agent.isStopped = false;


    }

    void CheckAttackHit()
    {
        if (hasHitPlayerThisAttack) return;

        Collider[] hits = Physics.OverlapBox(AttackBoxCenter, attackBoxSize / 2f, transform.rotation);
        foreach (Collider col in hits)
        {
            if (col.CompareTag("Player"))
            {
                hasHitPlayerThisAttack = true;
                Debug.Log("Acertei o player!");
                
                col.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth);
                playerHealth.TakeDamage(damage);

                break;
            }
        }
    }

    IEnumerator Appear()
    {
        yield return new WaitForSeconds(1f);
        animator.speed = 1;

        yield return new WaitForSeconds(1f);
        if (currentState == SkeletonState.Hide)
            onAppearAnimationEnd();
    }
    void onAppearAnimationEnd()
    {
        currentState = SkeletonState.Idle;
    }

    void onAttackAnimationEnd()
    {
        // entra em stun/cooldown (usa o timer que já existia no Idle)
        attackTimer = attackCooldown;
        isAttacking = false;
        currentState = SkeletonState.Idle;
        animator.SetBool("Attacking", false);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // desenha a hitbox de ataque (mesma posição/rotação usada no OverlapBox)
        Gizmos.color = Color.yellow;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(AttackBoxCenter, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, attackBoxSize);
        Gizmos.matrix = oldMatrix;
    }
}