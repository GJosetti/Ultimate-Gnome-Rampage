using UnityEngine;

public class PlayerController : MonoBehaviour
{


    [SerializeField]
    public bool IsDashAttacking;

    [SerializeField]
    public bool isAttackSpin;

    [SerializeField]
    public bool IsJumping;

    public void SetDashAttacking(bool value) => IsDashAttacking = value;
    public void SetJumping(bool value) => IsJumping = value;

    public void SetAttackSpin(bool value) => isAttackSpin = value;
}

