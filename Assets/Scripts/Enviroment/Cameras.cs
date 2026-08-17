using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class Cameras : MonoBehaviour
{
    [SerializeField] CinemachineCamera cineCamera;
    CinemachineBasicMultiChannelPerlin perlin;
    Coroutine shakeRoutine;

    [SerializeField]
    int myRoom;

    void Awake()
    {
        // pega o componente de Noise dentro da CinemachineCamera
        cineCamera = GetComponent<CinemachineCamera>();
        perlin = cineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }
    private void Update()
    {
        if (myRoom == GameManager.room)
        {
            GameManager.camera = this;
        }
    }
  

public void ShakeCamera(float intensity = 10f, float frequency = 2f, float duration = 0.3f)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(DoShake(intensity, frequency, duration));
    }

    IEnumerator DoShake(float intensity, float frequency, float duration)
    {
        perlin.AmplitudeGain = intensity;
        perlin.FrequencyGain = frequency;

        yield return new WaitForSeconds(duration);

        perlin.AmplitudeGain = 0f;
        perlin.FrequencyGain = 0f;
        shakeRoutine = null;
    }
}