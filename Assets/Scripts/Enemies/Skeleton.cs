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
    [SerializeField, Range(10, 40)] float attackForce;
    [SerializeField, Range(1, 20)] float attackCooldown;
    [SerializeField, Range(0.1f, 1f)] float windupOffset; //tempo de trava da rotação do inimigo até ele dar o dash 
    [SerializeField, Range(0.05f, 1f)] float attackDashDuration = 0.2f; // duração do "dash" em si

    [Header("Attack Hitbox")]
    [SerializeField] Vector3 attackBoxSize = new Vector3(1f, 1.5f, 1.5f);
    [SerializeField] float attackBoxForwardOffset = 1f;

    [SerializeField]
    float attackTimer = 0;
    Rigidbody rb;
    BoxCollider myCollider;
    NavMeshAgent agent;
    Animator animator;
    [SerializeField]
    SkeletonState currentState;
    bool isAttacking;
    bool hasHitPlayerThisAttack;
    Coroutine attackRoutineRef;

    bool playAudioAppear = false;
    [SerializeField] AudioSource skeletonAppear;

    enum SkeletonState
    {
        Hide,
        Idle,
        Walk,
        Attack,
        Hit,
        Die
    }

    // centro da hitbox de ataque: um pouco à frente do inimigo, na altura do corpo
    Vector3 AttackBoxCenter => transform.position + transform.forward * attackBoxForwardOffset + Vector3.up * (attackBoxSize.y / 2f);

    protected override void Start()
    {
        base.Start();

        animator = GetComponent<Animator>();
        animator.speed = 0;
        life = maxLife;
        currentState = SkeletonState.Hide;
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        myCollider = GetComponent<BoxCollider>();
        Physics.IgnoreCollision(myCollider, player.GetComponent<CapsuleCollider>(), true);
        rb.isKinematic = true;
        
    }
    // Lógica
    void Update()
    {
        if (PlayerController.Instance.isDead) return;

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
                    attackRoutineRef = StartCoroutine(AttackRoutine());
                }
                break;
            
            case SkeletonState.Die:
                break;

            case SkeletonState.Hit:

               
                break;
        }
    }
    //Física
    void FixedUpdate()
    {
        if (PlayerController.Instance.isDead) return;
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

        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.updatePosition = false;
        agent.updateRotation = false;

        animator.SetBool("Attacking", true);

        // windup (ainda kinemático, gira normalmente com MoveRotation)
        float windupTimer = 0f;
        Vector3 dashDir = transform.forward;
        while (windupTimer < attackStunTime)
        {
            Vector3 lookDir = GetPlayer().transform.position - transform.position;
            lookDir.y = 0;
            if (lookDir != Vector3.zero && windupTimer < attackStunTime - windupOffset)
            {
                dashDir = lookDir.normalized;
                rb.MoveRotation(Quaternion.LookRotation(dashDir));
            }
            windupTimer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // ativa física real só durante o dash
        rb.isKinematic = false;
        rb.AddForce(dashDir * attackForce, ForceMode.Impulse);

        float elapsed = 0f;
        while (elapsed < attackDashDuration)
        {
            CheckAttackHit();
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        agent.Warp(transform.position);
        agent.updatePosition = true;
        agent.updateRotation = true;
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
                
                
                col.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth);
                playerHealth.TakeDamage(damage);

                break;
            }
        }
    }



    void ResetAttackState()
    {
        isAttacking = false;
        hasHitPlayerThisAttack = false;

        // devolve o Rigidbody pro estado "seguro" (kinematic)
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // devolve o controle pro NavMeshAgent, caso tenha sido tirado no meio do dash
        agent.Warp(transform.position);
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.isStopped = false;

        animator.SetBool("Attacking", false);
        animator.SetBool("Running", false);
    }

    

    IEnumerator Appear()
    {
        yield return new WaitForSeconds(1f);
        animator.speed = 1;
        if (!playAudioAppear)
        {
            skeletonAppear.Play();
            playAudioAppear = true;
        }

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

    void onHitAnimationEnd()
    {
        currentState = SkeletonState.Idle;
    }

    public override void TakeDamage(int damage, Vector3 position)
    {
        if (currentState == SkeletonState.Hide) return;
        currentState = SkeletonState.Hit;

        if (attackRoutineRef != null)
        {
            StopCoroutine(attackRoutineRef);
            attackRoutineRef = null;
        }
        ResetAttackState();

        animator.Play("TakeDamage",0,0f);

        base.TakeDamage(damage, position);
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