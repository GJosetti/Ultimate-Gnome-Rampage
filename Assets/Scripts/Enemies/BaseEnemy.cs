using System.Collections;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public int maxLife = 100;
    int life;


    [SerializeField]
    MeshRenderer meshRenderer;
    Color originalColor;

    ParticleSystem pSystem;

    ParticleDamage particleDamage;

    void Start()
    {
        life = maxLife;
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.GetColor("_BaseColor");
        pSystem = GetComponentInChildren<ParticleSystem>();
        particleDamage = GetComponentInChildren<ParticleDamage>();
    }

    public void TakeDamage(int damage, Vector3 position)
    {
        life -= damage;
        Debug.Log($"Ai! Estou com {life} de vida");

        particleDamage.RotateHitEffect(position);
        pSystem.Play();

        StartCoroutine(FlashHit());

        if (life <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashHit()
    {
        meshRenderer.material.EnableKeyword("_EMISSION");
        meshRenderer.material.SetColor("_EmissionColor", Color.white * 3f);

        yield return new WaitForSeconds(0.15f);

        meshRenderer.material.SetColor("_EmissionColor", Color.black);
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} morreu");
        Destroy(gameObject);
    }
}