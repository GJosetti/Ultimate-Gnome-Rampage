using UnityEngine;

public class ParticleDamage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void RotateHitEffect( Vector3 playerPosition)
    {
        Vector3 dir = transform.position - playerPosition;
        dir.y = 0f; // ignora diferença de altura, só rotaciona no plano horizontal
        dir.Normalize();

        transform.rotation = Quaternion.LookRotation(dir);
    }
}
