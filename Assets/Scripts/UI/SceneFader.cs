using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] CanvasGroup fadeCanvasGroup; // Image preta full-screen dentro de um CanvasGroup
    [SerializeField] Button playButton;

    [Header("Configuração")]
    [SerializeField] string sceneToLoad;
    [SerializeField] float fadeDuration = 1f;

    void Start()
    {
        // garante que a tela começa transparente (visível o menu) e faz fade-in do preto pro menu
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 1f;
            fadeCanvasGroup.blocksRaycasts = false;
            StartCoroutine(Fade(1f, 0f, fadeDuration));
        }

        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayClicked);
        }
    }

    void OnPlayClicked()
    {
        playButton.interactable = false; // evita clique duplo durante a transição
        StartCoroutine(FadeOutAndLoadScene());
    }

    IEnumerator FadeOutAndLoadScene()
    {
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        SceneManager.LoadScene(sceneToLoad);
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        fadeCanvasGroup.blocksRaycasts = true; // bloqueia clique na UI de trás enquanto funde
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        fadeCanvasGroup.alpha = to;
        fadeCanvasGroup.blocksRaycasts = to > 0.5f; // só bloqueia se ficou opaco (preto cobrindo tudo)
    }
}