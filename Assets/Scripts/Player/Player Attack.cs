using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Range(0, 100)]
    public float dashDistance;
    public float dashDuration;
    [Range(1, 10)]  // mudei o mínimo pra 1, pra evitar divisão por zero
    public int tickRate;
    public int damagePerTick;
    public float attackRadius;
    [SerializeField]
    LayerMask enemyLayerMask;

    // Buffer reutilizável, evita alocar um array novo toda hora
    Collider[] hitBuffer = new Collider[20]; // 20 = número máximo de inimigos detectados por vez, ajuste se precisar

    Rigidbody rb;
    PlayerController controller;
    PlayerRotation rotation;

    
    SwordDrag swordDrag;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<PlayerController>();
        rotation = GetComponent<PlayerRotation>();
        swordDrag = GetComponentInChildren<SwordDrag>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!controller.IsDashAttacking)
            {
                StartCoroutine(DashAttack(rotation.mouseDir));
            }
        }
    }

    IEnumerator DashAttack(Vector3 dir)
    {
        controller.SetDashAttacking(true);
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero; // zera qualquer rotação "fantasma" acumulada
        rb.AddForce(dir * dashDistance, ForceMode.Impulse);

        swordDrag?.ResetDrag();

        float rotated = 0f;
        float rotationSpeed = 360f / dashDuration;

        while (rotated < 360f)
        {
            yield return new WaitForFixedUpdate(); // sincroniza com o passo de física
            float step = rotationSpeed * Time.fixedDeltaTime;
            step = Mathf.Min(step, 360f - rotated);
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, step, 0f));
            rotated += step;
        }

        yield return StartCoroutine(SpinAttack());
        controller.SetDashAttacking(false);
    }

    IEnumerator SpinAttack()
    {
        float rotationSpeed = 360f / dashDuration;
        controller.SetAttackSpin(true);
        StartCoroutine(TickDamage());

        while (Input.GetMouseButton(0))
        {
            yield return new WaitForFixedUpdate();
            float step = rotationSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, step, 0f));
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        controller.SetAttackSpin(false);
    }

    IEnumerator TickDamage()
    {
        while (controller.isAttackSpin)
        {
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, attackRadius, hitBuffer, enemyLayerMask);

            for (int i = 0; i < hitCount; i++)
            {
                if (hitBuffer[i].TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
                {
                    enemy.TakeDamage(damagePerTick);
                }
            }

            yield return new WaitForSeconds(1f / tickRate);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}