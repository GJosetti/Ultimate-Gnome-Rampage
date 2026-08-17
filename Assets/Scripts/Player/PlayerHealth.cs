using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    public int maxHealth, actualHealth;
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        actualHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (actualHealth <= 0)
        {
            GameManager.ResetState();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void TakeDamage(int amount)
    {
        actualHealth -= amount;
        GameManager.camera.ShakeCamera();
    }
}
