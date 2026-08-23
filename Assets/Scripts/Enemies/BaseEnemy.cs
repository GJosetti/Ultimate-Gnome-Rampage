using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{

    public event System.Action<BaseEnemy> OnDeath;

    [SerializeField]
    protected int maxLife = 3;
    protected int life;
    [SerializeField]
    protected int myRoom;

    [SerializeField]
    protected PlayerController player;

    Renderer[] meshRenderers;
    Material[] materials;

    ParticleSystem pSystem;
    ParticleDamage particleDamage;

    [SerializeField]
    ParticleSystem diePSystem;

    [SerializeField]
    List<PowerUp> listPowerUps;

    protected virtual void Start()
    {
        player = PlayerController.Instance;

        life = maxLife;

        meshRenderers = GetComponentsInChildren<Renderer>();
       
        materials = new Material[meshRenderers.Length];
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            materials[i] = meshRenderers[i].material; // instancia, evita mexer no shared material
        }
       

        pSystem = GetComponentInChildren<ParticleSystem>();
        //diePSystem = GetComponentInChildren<ParticleSystem>();
        particleDamage = GetComponentInChildren<ParticleDamage>();
    }

    protected PlayerController GetPlayer()
    {
       
            if (player == null && PlayerController.Instance != null)
                player = PlayerController.Instance;
            return player;
        
    }






    public virtual void TakeDamage(int damage, Vector3 position)
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
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].EnableKeyword("_EMISSION");
            materials[i].SetColor("_EmissionColor", Color.white * 3f);
        }

        yield return new WaitForSeconds(0.15f);

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetColor("_EmissionColor", Color.black);
        }
    }

    void Die()
    {
        OnDeath?.Invoke(this);

        diePSystem.transform.parent = null; // desgruda do esqueleto
        diePSystem.Play();

        if (listPowerUps.Count > 0)
        { 
            Instantiate(listPowerUps[Random.Range(0, listPowerUps.Count)],new Vector3(transform.position.x, 1.5f,transform.position.z),Quaternion.identity);        
        }
        
        Destroy(gameObject);
        Destroy(diePSystem.gameObject, diePSystem.main.duration);
    }
}