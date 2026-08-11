using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{

    [Range(0, 100)]
    public float dashDistance;
    public float dashDuration;


    public bool spin = true;

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

        if (Input.GetMouseButtonDown(0))
        { 
            if (!controller.IsDashAttacking)
            {
                StartCoroutine(DashAttack(rotation.mouseDir));
            }
            
        }
    }


    IEnumerator DashAttack(Vector3 dir)
    {
        controller.SetDashAttacking(true);
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(dir * dashDistance, ForceMode.Impulse);

        float rotated = 0f;
        float rotationSpeed = (360/dashDuration)/dashDuration; 

        
        //Primeiro Ataque de Dash
        while (rotated < 360)
        {
            float step = rotationSpeed * Time.deltaTime;
            step = Mathf.Min(step, 360f - rotated); 
            transform.Rotate(Vector3.up, step);
            rotated += step;
            yield return null;
        }


        while(Input.GetMouseButton(0)) //Ataque com Spin
        {
            controller.SetAttackSpin(true);
            float step = rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, step);
            rotated += step;
            yield return null;

        }
        SpinAttack();

        
        controller.SetDashAttacking(false);
    }

    void SpinAttack()
    {
        float rotated = 0f;
        float rotationSpeed = (360 / dashDuration) / dashDuration;

        while (Input.GetMouseButton(0)) //Ataque com Spin
        {
            controller.SetAttackSpin(true);
            float step = rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, step);
            rotated += step;
          

        }

        controller.SetAttackSpin(false);
    }








}
