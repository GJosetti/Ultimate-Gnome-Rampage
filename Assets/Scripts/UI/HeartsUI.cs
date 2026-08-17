using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartsUI : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] Image heartPrefab;      // prefab com o sprite de coração já configurado
    [SerializeField] Transform heartsParent; // objeto com Horizontal Layout Group (ex: um painel vazio na UI)

    List<Image> hearts = new List<Image>();
    int lastLifeShown = -1; // controle pra só redesenhar quando a vida realmente mudar
    [SerializeField]
    PlayerHealth playerHealth;

    void Update()
    {
        int currentLife = playerHealth.actualHealth;

        if (currentLife != lastLifeShown)
        {
            UpdateHearts(currentLife);
            lastLifeShown = currentLife;
        }
    }

    void UpdateHearts(int currentLife)
    {
        // garante que exista um coração pra cada ponto de vida
        while (hearts.Count < currentLife)
        {
            Image newHeart = Instantiate(heartPrefab, heartsParent);
            hearts.Add(newHeart);
        }

        // remove corações extras, se a vida máxima tiver diminuído
        while (hearts.Count > currentLife && hearts.Count > 0)
        {
            Image lastHeart = hearts[hearts.Count - 1];
            hearts.RemoveAt(hearts.Count - 1);
            Destroy(lastHeart.gameObject);
        }
    }
}