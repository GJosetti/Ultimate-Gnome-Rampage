using UnityEngine;

public class PlayerController : MonoBehaviour
{


    public static PlayerController Instance { get; private set; }

    [SerializeField]
    public bool IsDashAttacking;

    [SerializeField]
    public bool isAttackSpin;

    [SerializeField]
    public bool IsJumping;

    [SerializeField]
    public bool IsAttacking;

    public void SetDashAttacking(bool value) => IsDashAttacking = value;
    public void SetJumping(bool value) => IsJumping = value;

    public void SetAttackSpin(bool value) => isAttackSpin = value;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        IsAttacking = isAttackSpin || IsDashAttacking;

    }
}

