using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("Morte")]
    [SerializeField] Animator animator;
    [SerializeField] Image redFilter;
    [SerializeField] CanvasGroup deathScreen;
    [SerializeField] Button restartButton;
    [SerializeField] float redFilterFadeDuration = 1f;
    [SerializeField] float deathScreenDelay = 1.5f;

    [SerializeField] AudioSource takedDamageAudio;
    [SerializeField] AudioSource gainLifeAudio;


    bool isDead;

    void Start()
    {
        actualHealth = maxHealth;

        if (meshRenderer != null)
        {
            meshMaterial = meshRenderer.material;
            originalColor = meshMaterial.GetColor(BaseColorID);
            meshMaterial.EnableKeyword("_EMISSION");
        }

        if (deathScreen != null)
        {
            deathScreen.alpha = 0f;
            deathScreen.gameObject.SetActive(false);
        }
        if (redFilter != null)
        {
            Color c = redFilter.color;
            c.a = 0f;
            redFilter.color = c;
        }
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartLevel);
        }
    }

    void Update()
    {
        if (iFrameTimer > 0)
        {
            iFrameTimer -= Time.deltaTime;
        }

        UpdateInvincibleBlink();

        if (actualHealth <= 0 && !isDead)
        {
            isDead = true;
            PlayerController.Instance.SetIsDead(true);
            StartCoroutine(DeathSequence());
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
            meshMaterial.SetColor(EmissionColorID, Color.black);
        }
    }

    IEnumerator DeathSequence()
    {
        animator?.SetTrigger("Die");
        StartCoroutine(FadeRedFilter());

        yield return new WaitForSeconds(deathScreenDelay);

        if (deathScreen != null)
        {
            deathScreen.gameObject.SetActive(true);
            yield return StartCoroutine(FadeCanvasGroup(deathScreen, 0f, 1f, 0.5f));
        }
    }

    IEnumerator FadeRedFilter()
    {
        if (redFilter == null) yield break;

        float elapsed = 0f;
        Color c = redFilter.color;
        while (elapsed < redFilterFadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 0.6f, elapsed / redFilterFadeDuration);
            redFilter.color = c;
            yield return null;
        }
    }

    IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        group.alpha = to;
        group.interactable = to > 0.5f;
        group.blocksRaycasts = to > 0.5f;
    }

    void RestartLevel()
    {
        GameManager.ResetState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TakeDamage(int amount)
    {
        if (iFrameTimer <= 0 && !PlayerController.Instance.IsInvencible)
        {
            takedDamageAudio.Play();
            actualHealth -= amount;
            GameManager.camera.ShakeCamera();
            iFrameTimer = IFrameDuration;
           
        }
    }

    public void IncreaseHealth(int amount)
    {
        actualHealth += amount;
        gainLifeAudio.Play();
    }
}