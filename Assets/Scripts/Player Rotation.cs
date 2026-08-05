using UnityEngine;

public class PlayerRotation : MonoBehaviour
{

    Vector3 mousePos;
    Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Input.mousePosition;

        int layerMask = ~LayerMask.GetMask("UI");

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            Vector3 target = hit.point;

            target.y = transform.position.y;

            Vector3 direction = target - transform.position;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
}
