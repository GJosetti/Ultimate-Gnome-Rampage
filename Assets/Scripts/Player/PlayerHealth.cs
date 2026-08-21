using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    public int maxHealth, actualHealth;
    public float IFrameDuration;
    float iFrameTimer;
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        actualHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

        if (iFrameTimer > 0)
        { 
            iFrameTimer -= Time.deltaTime;
        }
        
        if (actualHealth <= 0)
        {
            GameManager.ResetState();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void TakeDamage(int amount)
    {
        if (iFrameTimer <= 0)
        { 
            actualHealth -= amount;
            GameManager.camera.ShakeCamera();
            iFrameTimer = IFrameDuration;
        }
    }
}
