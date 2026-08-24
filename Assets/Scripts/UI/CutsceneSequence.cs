using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneSequence : MonoBehaviour
{
    [Header("Imagens da Cutscene")]
    [SerializeField] Image displayImage; // Image full-screen que vai trocar de sprite
    [SerializeField] Sprite[] cutsceneImages;
    [SerializeField] float timePerImage = 3f;
    [SerializeField] float fadeDuration = 0.75f;

    [Header("Transição pra Gameplay")]
    [SerializeField] string gameplaySceneName;
    [SerializeField] CanvasGroup blackFadeCanvasGroup; // tela preta full-screen separada, pra fade final

    [Header("Skip (opcional)")]
    [SerializeField] bool allowSkip = true;

    Coroutine sequenceRoutine;

    void Start()
    {
        if (blackFadeCanvasGroup != null)
        {
            blackFadeCanvasGroup.alpha = 0f;
            blackFadeCanvasGroup.blocksRaycasts = false;
        }

        sequenceRoutine = StartCoroutine(PlayCutscene());
    }

    void Update()
    {
        if (allowSkip && sequenceRoutine != null && Input.GetKeyDown(KeyCode.Space))
        {
            StopCoroutine(sequenceRoutine);
            GoToGameplay();
        }
    }

    IEnumerator PlayCutscene()
    {
        for (int i = 0; i < cutsceneImages.Length; i++)
        {
            displayImage.sprite = cutsceneImages[i];

            // fade-in da imagem atual
            yield return StartCoroutine(FadeImageAlpha(0f, 1f, fadeDuration));

            // segura na tela pelo tempo configurado (já descontando o fade-in, se preferir tempo total fixo por imagem)
            yield return new WaitForSeconds(timePerImage);

            // fade-out antes de trocar pra próxima (exceto na última, que faz fade pro preto direto)
            if (i < cutsceneImages.Length - 1)
            {
                yield return StartCoroutine(FadeImageAlpha(1f, 0f, fadeDuration));
            }
        }

        GoToGameplay();
    }

    IEnumerator FadeImageAlpha(float from, float to, float duration)
    {
        Color c = displayImage.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / duration);
            displayImage.color = c;
            yield return null;
        }

        c.a = to;
        displayImage.color = c;
    }

    void GoToGameplay()
    {
        StartCoroutine(FinalFadeAndLoad());
    }

    IEnumerator FinalFadeAndLoad()
    {
        if (blackFadeCanvasGroup != null)
        {
            blackFadeCanvasGroup.blocksRaycasts = true;
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                blackFadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                yield return null;
            }
            blackFadeCanvasGroup.alpha = 1f;
        }

        SceneManager.LoadScene(gameplaySceneName);
    }
}