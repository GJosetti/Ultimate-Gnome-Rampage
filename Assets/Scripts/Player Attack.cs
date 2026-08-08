using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{

    [Range(0, 100)]
    public float dashDistance;

    Rigidbody rb;

    PlayerController controller;

    PlayerRotation rotation;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();   
        controller = GetComponent<PlayerController>();
        rotation = GetComponent<PlayerRotation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!controller.IsAttacking && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(DashAttack(rotation.mouseDir));
        }
    }


    void InputCheck()
    {
        if (Input.GetMouseButtonDown(0))
        {

        }
;
    }

    IEnumerator DashAttack(Vector3 dir)
    {
        //Para caso esteja pulando no meio do movimento, para dar o dash em sequencia
        controller.SetAttacking(true);
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(dir * dashDistance, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5F);
        controller.SetAttacking(false);
    }
}
