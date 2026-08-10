using System.Collections;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{

    public Vector3 mouseDir;
    Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        int layerMask = ~LayerMask.GetMask("UI");

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            Vector3 target = hit.point;

            target.y = transform.position.y;

            Vector3 direction = target - transform.position;

            mouseDir = direction.normalized;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

}
