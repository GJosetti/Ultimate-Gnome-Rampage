using UnityEngine;

public class Cannon : BaseEnemy
{
    [Header("Referências")]
    [SerializeField] Transform cannonBarrel; // parte que rotaciona, filho separado da base
    [SerializeField] Transform firePoint;    // ponto na ponta do cano de onde o projétil vai sair (opcional, pra quando implementar)
    [SerializeField] GameObject projectilePrefab; // prefab do projétil (opcional, pra quando implementar)

    [Header("Mira")]
    [SerializeField, Range(0.1f, 5f)] float aimTrackDuration = 1.5f; // tempo total seguindo o player antes de travar
    [SerializeField, Range(0f, 2f)] float aimLockOffset = 0.3f;      // trava a mira X segundos antes do fim do tracking
    [SerializeField, Range(1f, 30f)] float aimRotationSpeed = 180f;  // graus/segundo do giro do cano

    [Header("Idle")]
    [SerializeField, Range(0.5f, 10f)] float idleDuration = 2f;


    Animator animator;
    [SerializeField]
    CannonState currentState;

    float aimTimer;
    float idleTimer;
    bool aimLocked;

    enum CannonState
    {
        Hide,
        Appear,
        Shooting,
        Idle
    }

    protected override void Start()
    {
        base.Start();

        animator = GetComponent<Animator>();
        life = maxLife;
        currentState = CannonState.Hide;
    }

    void Update()
    {
        switch (currentState)
        {
            case CannonState.Hide:
                if (myRoom == GameManager.room)
                {
                    EnterAppear();
                }
                break;

            case CannonState.Appear:
                // nada aqui; a troca de estado acontece via onAppearAnimationEnd (Animation Event)
                break;

            case CannonState.Shooting:
                UpdateAiming();
                break;

            case CannonState.Idle:
                idleTimer -= Time.deltaTime;
                if (idleTimer <= 0f)
                {
                    EnterShooting();
                }
                break;
        }
    }

    void EnterAppear()
    {
        currentState = CannonState.Appear;
        animator.SetTrigger("Appear");
    }

    void EnterShooting()
    {
        currentState = CannonState.Shooting;
        aimTimer = 0f;
        aimLocked = false;
        // não dispara "Shoot" aqui — o cano fica parado (Idle) enquanto mira
    }

    void UpdateAiming()
    {
        if (aimLocked) return;

        Vector3 dirToPlayer = GetPlayer().transform.position - cannonBarrel.position;
        dirToPlayer.y = 0;
        if (dirToPlayer != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dirToPlayer);
            cannonBarrel.rotation = Quaternion.RotateTowards(cannonBarrel.rotation, targetRot, aimRotationSpeed * Time.deltaTime);
        }

        aimTimer += Time.deltaTime;
        if (aimTimer >= aimTrackDuration - aimLockOffset)
        {
            aimLocked = true;
            animator.SetTrigger("Shoot"); // só agora, com a mira já travada, dispara a animação de tiro
        }
    }

    // chamado via Animation Event, no frame exato em que o cano deveria soltar o projétil
    void FireProjectile()
    {
        Instantiate(projectilePrefab, firePoint.position, cannonBarrel.rotation);
        // TODO: instanciar/ativar o projétil aqui quando estiver pronto
        // ex: Instantiate(projectilePrefab, firePoint.position, cannonBarrel.rotation);
    }

    // chamado via Animation Event, no fim da animação de aparecer
    void onAppearAnimationEnd()
    {
        EnterShooting();
    }

    // chamado via Animation Event, no fim da animação de atirar
    void onShootAnimationEnd()
    {
        currentState = CannonState.Idle;
        idleTimer = idleDuration;
    }

    private void OnDrawGizmos()
    {
        if (cannonBarrel == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(cannonBarrel.position, cannonBarrel.forward * 5f);
    }
}