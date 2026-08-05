using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class PlayerMovement : MonoBehaviour
{
    
    Rigidbody rb;

    [SerializeField]
    bool isJumping;

    [Range(0,100)]
    public float jumpForce, jumpDistance, footHeight;




    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame

    

    void Update()
    {
        Vector3 inputDir = InputCheck();
    
        if (inputDir == Vector3.zero) return;
        
        if (!isJumping && isOnGround())
        {
            JumpTowardsDirection(inputDir);
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


    IEnumerator  JumpSequence(Vector3 dir)
    {
        isJumping = true;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        
        yield return new WaitForSeconds(0.1f);

        rb.AddForce(dir * jumpDistance, ForceMode.Impulse);

        yield return new WaitForSeconds(0.2f);

        isJumping = false;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * footHeight);
    }


}
