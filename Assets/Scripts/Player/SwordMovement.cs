using System.Collections.Generic;
using UnityEngine;

public class SwordDrag : MonoBehaviour
{

    [SerializeField]
    PlayerController controller;
    
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

    public BoxCollider collider;

    private HashSet<BaseEnemy> hitEnemies = new HashSet<BaseEnemy>();
    void Start()
    {
        currentPos = handAnchor.position;
        collider = GetComponent<BoxCollider>();
    }

    void FixedUpdate()
    {
    
        Vector3 toTarget = handAnchor.position - currentPos;
        Vector3 acceleration = toTarget * stiffness - velocity * damping;
        velocity += acceleration * Time.fixedDeltaTime;

        if (toTarget.magnitude < 0.1f)
        {
            velocity = Vector3.zero;
        }

        currentPos += velocity * Time.fixedDeltaTime;

    
        Vector3 diff = currentPos - handAnchor.position;
        if (diff.magnitude > maxLag)
        {
            Vector3 clampDir = diff.normalized;
            currentPos = handAnchor.position + clampDir * maxLag;

            
            float radialVel = Vector3.Dot(velocity, clampDir);
            if (radialVel > 0)
                velocity -= clampDir * radialVel;
        }

      
        Vector2 flatOffset = new Vector2(currentPos.x - playerCenter.position.x,
                                          currentPos.z - playerCenter.position.z);

        if (flatOffset.magnitude < playerRadius)
        {
            Vector2 pushDir = flatOffset.normalized;

            if (flatOffset.magnitude < 0.001f)
                pushDir = Vector2.up;

            Vector2 correctedFlat = pushDir * playerRadius;
            currentPos.x = playerCenter.position.x + correctedFlat.x;
            currentPos.z = playerCenter.position.z + correctedFlat.y;

            Vector3 correctionDir = new Vector3(pushDir.x, 0, pushDir.y);
            float velocityIntoPlayer = Vector3.Dot(velocity, -correctionDir);
            if (velocityIntoPlayer > 0)
                velocity += correctionDir * velocityIntoPlayer;
        }

        transform.position = currentPos;

        // --- Rotaciona a espada na direção da mão ---
        Vector3 dir = handAnchor.position - currentPos;
        if (dir.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(90, angle + 90, 0);
        }



    }


    public void ResetHits()
    {
        hitEnemies.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
        {
            if (hitEnemies.Contains(enemy)) return; // já foi atingido nesse ataque

            hitEnemies.Add(enemy);
            enemy.TakeDamage(1, transform.position);
        }
    }
    public void ResetDrag()
    {
        currentPos = handAnchor.position;
        velocity = Vector3.zero;
    }

}