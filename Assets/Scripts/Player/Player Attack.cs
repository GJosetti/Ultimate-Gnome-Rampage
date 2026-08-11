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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<PlayerController>();
        rotation = GetComponent<PlayerRotation>();
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
        rb.AddForce(dir * dashDistance, ForceMode.Impulse);
        float rotated = 0f;
        float rotationSpeed = (360f) / dashDuration; // corrigido: era dividido duas vezes

        while (rotated < 360f)
        {
            float step = rotationSpeed * Time.deltaTime;
            step = Mathf.Min(step, 360f - rotated);
            transform.Rotate(Vector3.up, step);
            rotated += step;
            yield return null;
        }

        yield return StartCoroutine(SpinAttack());
        controller.SetDashAttacking(false);
    }

    IEnumerator SpinAttack()
    {
        float rotated = 0f;
        float rotationSpeed = 360f / dashDuration; // corrigido também aqui
        controller.SetAttackSpin(true);
        StartCoroutine(TickDamage());

        while (Input.GetMouseButton(0))
        {
            float step = rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, step);
            rotated += step;
            yield return null;
        }
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