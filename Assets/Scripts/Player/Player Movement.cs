using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class PlayerMovement : MonoBehaviour
{
    
    Rigidbody rb;

  
    

    [Range(0,100)]
    public float jumpForce, jumpDistance, footHeight,walkSpeed;

    PlayerController controller;


    void Start()
    {
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame

    

    void FixedUpdate()
    {

        if (controller.IsDashAttacking && !controller.isAttackSpin) return; // não pode mover enquanto ataca

        Vector3 inputDir = InputCheck();

        if (controller.isAttackSpin)
        {
            rb.AddForce(inputDir * walkSpeed, ForceMode.Force);
        }
        else
        { 
            if (!controller.IsJumping && isOnGround() && (inputDir != Vector3.zero))
            {
                JumpTowardsDirection(inputDir);
            }
        }

    }


    Vector3 InputCheck()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        return new Vector3 (horizontalInput, 0, verticalInput);
    }

    void JumpTowardsDirection(Vector3 dir)
    {
        StartCoroutine(JumpSequence(dir));
    }

    bool isOnGround()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit, footHeight);

        if (hit.collider == null)
        {
            return false;
        }

        if (hit.collider.gameObject.layer == 6)
        { 
            return true;
        }
        return false;

    }


    IEnumerator JumpSequence(Vector3 dir)
    {
        controller.SetJumping(true);
        
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        
        yield return new WaitForSeconds(0.1f);

        rb.AddForce(dir * jumpDistance, ForceMode.Impulse);

        yield return new WaitForSeconds(0.2f);

        controller.SetJumping(false);

    }




    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * footHeight);
    }


}
