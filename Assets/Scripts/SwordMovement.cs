using UnityEngine;

public class SwordDrag : MonoBehaviour
{
    [Header("Referências")]
    public Transform handAnchor;
    public Transform playerCenter;    // centro do player (geralmente o pivot do corpo)

    [Header("Física do arraste")]
    public float stiffness = 8f;
    public float damping = 4f;
    public float maxLag = 1.5f;

    [Header("Colisão com o corpo")]
    public float playerRadius = 0.6f; // raio aproximado do corpo do player (visto de cima)

    private Vector3 velocity;
    private Vector3 currentPos;

    void Start()
    {
        currentPos = handAnchor.position;
    }

    void FixedUpdate()
    {
        // --- Spring normal ---
        Vector3 toTarget = handAnchor.position - currentPos;
        Vector3 acceleration = toTarget * stiffness - velocity * damping;
        velocity += acceleration * Time.fixedDeltaTime;
        if (toTarget.magnitude < 0.1)
        {
            velocity = Vector3.zero;
        }
        currentPos += velocity * Time.fixedDeltaTime;

        Vector3 diff = currentPos - handAnchor.position;
        if (diff.magnitude > maxLag)
            currentPos = handAnchor.position + diff.normalized * maxLag;

        // --- Empurra a espada pra fora do corpo do player ---
        Vector2 flatOffset = new Vector2(currentPos.x - playerCenter.position.x,
                                          currentPos.z - playerCenter.position.z);

        if (flatOffset.magnitude < playerRadius)
        {
            Vector2 pushDir = flatOffset.normalized;
            // Se a espada estiver exatamente no centro (raro), empurra numa direção padrão
            if (flatOffset.magnitude < 0.001f)
                pushDir = Vector2.up;

            Vector2 correctedFlat = pushDir * playerRadius;
            currentPos.x = playerCenter.position.x + correctedFlat.x;
            currentPos.z = playerCenter.position.z + correctedFlat.y;

            // Zera a velocidade na direção da correção pra não "empurrar de volta" com força
            Vector3 correctionDir = new Vector3(pushDir.x, 0, pushDir.y);
            float velocityIntoPlayer = Vector3.Dot(velocity, -correctionDir);
            if (velocityIntoPlayer > 0)
                velocity += correctionDir * velocityIntoPlayer;
        }

        transform.position = currentPos;

        // --- Rotação (yaw only, top-down) ---
        Vector3 dir = handAnchor.position - currentPos;
        if (dir.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(90, angle, 0);
        }
    }
}