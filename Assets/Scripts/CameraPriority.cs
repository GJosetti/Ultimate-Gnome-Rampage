using Unity.Cinemachine;
using UnityEngine;

public class CameraPriority : MonoBehaviour
{
    [SerializeField] int defaultPriority;
    CinemachineCamera cam;

    void Awake()
    {
        cam = GetComponent<CinemachineCamera>();
        cam.Priority = defaultPriority;
    }
}
