using Unity.Cinemachine;
using UnityEngine;
public class Door : MonoBehaviour
{
    [SerializeField]
    CinemachineCamera camera1, camera2;
    [SerializeField]
    int myRoom;
    BoxCollider collider;
    Animator portaAnim;
    [SerializeField]
    SceneFader fade;

    Vector3 EntradaPos;

    void Start()
    {
        collider = GetComponent<BoxCollider>();
        portaAnim = GetComponentInChildren<Animator>();
        portaAnim.speed = 0;
        if (myRoom != 0)
        {
            portaAnim.SetBool("isOpen", false);
            portaAnim.speed = 1;
            collider.isTrigger = false;
        }
    }

    void Update()
    {
        if (myRoom != 0)
        {
            // sincroniza o estado nos dois sentidos (abre E fecha),
            // em vez de só abrir - evita ficar "preso" aberto por causa
            // de um valor de inCombat momentaneamente desatualizado
            bool shouldBeOpen = !GameManager.inCombat && GameManager.room == myRoom;
            portaAnim.SetBool("isOpen", shouldBeOpen);
            collider.isTrigger = shouldBeOpen;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && (EntradaPos.x < other.gameObject.transform.position.x))
        {

            if (myRoom == 6)
            {
                fade.onFinalDoorReached();
            }
            
            
            
            
            
            camera1.Priority = 0;
            camera2.Priority = 10;
            GameManager.room++;

            // assume que a nova sala começa em combate até a Room dela
            // avaliar os inimigos e dizer o contrário - fecha a janela
            // de 1 frame onde inCombat ainda tinha o valor da sala antiga
            GameManager.inCombat = true;

            collider.isTrigger = false;
            portaAnim.SetBool("isOpen", false);
            portaAnim.speed = 1;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EntradaPos = other.transform.position;
            
        }
    }
}