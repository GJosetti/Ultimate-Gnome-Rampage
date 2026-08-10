using UnityEngine;

public class PlayerController : MonoBehaviour
{


    [SerializeField]
    public bool IsAttacking;

    [SerializeField]
    public bool isAttackRotating;

    [SerializeField]
    public bool IsJumping;

    public void SetAttacking(bool value) => IsAttacking = value;
    public void SetJumping(bool value) => IsJumping = value;
}
