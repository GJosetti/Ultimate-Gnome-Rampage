using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    public int maxHealth, actualHealth;
    public float IFrameDuration;
    float iFrameTimer;

    [Header("Invencibilidade Visual")]
    [SerializeField] Renderer meshRenderer;
    [SerializeField] Color invincibleColor = Color.yellow;
    [SerializeField] Color emissionColor = Color.yellow;
    [SerializeField, Range(0f, 5f)] float emissionIntensity = 2f;
    [SerializeField, Range(1f, 30f)] float blinkSpeed = 10f;

    Color originalColor;
    Material meshMaterial;

    static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    void Start()
    {
        actualHealth = maxHealth;

        if (meshRenderer != null)
        {
            meshMaterial = meshRenderer.material;
            originalColor = meshMaterial.GetColor(BaseColorID);
            meshMaterial.EnableKeyword("_EMISSION"); // habilita emissão desde já; controlamos a intensidade via cor
        }
    }

    void Update()
    {
        if (iFrameTimer > 0)
        {
            iFrameTimer -= Time.deltaTime;
        }

        UpdateInvincibleBlink();

        if (actualHealth <= 0)
        {
            GameManager.ResetState();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void UpdateInvincibleBlink()
    {
        if (meshMaterial == null) return;

        if (PlayerController.Instance.IsInvencible)
        {
            float t = (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f;

            meshMaterial.SetColor(BaseColorID, Color.Lerp(originalColor, invincibleColor, t));
            meshMaterial.SetColor(EmissionColorID, emissionColor * emissionIntensity * t);
        }
        else
        {
            meshMaterial.SetColor(BaseColorID, originalColor);
            meshMaterial.SetColor(EmissionColorID, Color.black); // emissão "zerada" = sem brilho
        }
    }

    public void TakeDamage(int amount)
    {
        if (iFrameTimer <= 0 && !PlayerController.Instance.IsInvencible)
        {
            actualHealth -= amount;
            GameManager.camera.ShakeCamera();
            iFrameTimer = IFrameDuration;
        }
    }

    public void IncreaseHealth(int amount)
    {
        actualHealth += amount;
    }
}