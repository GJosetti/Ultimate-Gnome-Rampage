using UnityEngine;

public class PowerUpFloating : MonoBehaviour
{
    [Header("Flutuação")]
    [SerializeField] float floatAmplitude = 0.25f; // o quanto sobe/desce
    [SerializeField] float floatSpeed = 2f;         // velocidade da oscilação

    [Header("Rotação")]
    [SerializeField] float rotationSpeed = 90f; // graus por segundo, eixo Y

    Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }
}