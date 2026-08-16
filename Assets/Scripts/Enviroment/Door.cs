using Unity.Cinemachine;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField]
    CinemachineCamera camera1, camera2;

   

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           camera1.Priority = 0;
           camera2.Priority = 10;
            GameManager.room++;
        }
    }
}
