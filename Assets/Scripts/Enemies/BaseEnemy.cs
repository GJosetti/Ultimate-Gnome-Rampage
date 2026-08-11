using System.Collections;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public int maxLife = 100;
    int life;

    [SerializeField]
    MeshRenderer meshRenderer;
    Color originalColor;

    void Start()
    {
        life = maxLife;
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.GetColor("_BaseColor");
    }

    public void TakeDamage(int damage)
    {
        life -= damage;
        Debug.Log($"Ai! Estou com {life} de vida");

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