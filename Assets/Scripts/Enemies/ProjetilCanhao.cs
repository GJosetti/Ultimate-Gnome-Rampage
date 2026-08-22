using UnityEngine;

public class ProjetilCanhao : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField]
    float speed;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = transform.forward * speed;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(1);
        }
        if (!other.gameObject.CompareTag("Enemy"))
        { 
            Destroy(gameObject);
        }
    }
}
