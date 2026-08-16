using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Range(0, 100)]
    public float dashDistance;
    public float dashDuration;
    [Range(1, 10)]  // mudei o m�nimo pra 1, pra evitar divis�o por zero
    public int tickRate;
    [Range(0.05f, 5f)]
    public float spinSpeed;
    public int damagePerTick;
    public float attackRadius;
    [SerializeField]
    LayerMask enemyLayerMask;


    // Buffer reutiliz�vel, evita alocar um array novo toda hora
    Collider[] hitBuffer = new Collider[20]; // 20 = n�mero m�ximo de inimigos detectados por vez, ajuste se precisar

    Rigidbody rb;
    PlayerController controller;
    PlayerRotation rotation;


    [SerializeField]
    SwordDrag swordDrag;


    public PhysicsMaterial dashMaterial; // atrito 0, configurado no Inspector
    public PhysicsMaterial normalMaterial; // seu material padr�o
    Collider playerCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<PlayerController>();
        rotation = GetComponent<PlayerRotation>();
        
        playerCollider = GetComponent<Collider>();
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
        playerCollider.material = dashMaterial;
        controller.SetDashAttacking(true);
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero; // zera qualquer rota��o "fantasma" acumulada
        swordDrag.ResetHits();

        float ogLag = swordDrag.maxLag; 

        swordDrag.maxLag = 0;


        rb.AddForce(dir * dashDistance, ForceMode.Impulse);



        swordDrag?.ResetDrag();
        SetCollisionWithEnemies(false);

        float rotated = 0f;
        float rotationSpeed = 360f / dashDuration;
      
        while (rotated < 360f)
        {
            yield return new WaitForFixedUpdate(); // sincroniza com o passo de f�sica
            float step = rotationSpeed * Time.fixedDeltaTime;
            step = Mathf.Min(step, 360f - rotated);
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, step, 0f));
            rotated += step;
        }
        SetCollisionWithEnemies(true);
        playerCollider.material = normalMaterial;

        if (Input.GetMouseButton(0))
        {
            Debug.Log("Não soltou");
            yield return StartCoroutine(SpinAttack());    
        }
        swordDrag.maxLag = ogLag;
        controller.SetDashAttacking(false);

    }

    IEnumerator SpinAttack()
    {
        float rotationSpeed = 360f / spinSpeed;
        controller.SetAttackSpin(true);
       
        while (Input.GetMouseButton(0))
        {
            yield return new WaitForFixedUpdate();
            swordDrag.ResetHits();
            float step = rotationSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, step, 0f));
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        controller.SetAttackSpin(false);
    }

    void SetCollisionWithEnemies(bool enabled)
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, 5f, enemyLayerMask);
        Collider playerCol = GetComponent<Collider>();
        foreach (Collider enemyCol in enemies)
        {
            Physics.IgnoreCollision(playerCol, enemyCol, !enabled);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}