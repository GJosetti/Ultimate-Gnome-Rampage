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

    [SerializeField]
    public bool IsInvencible;


    [SerializeField]
    public bool IsFastSpin;


    [SerializeField]
    public float FastSpinDuration;
    [SerializeField]
    public float FastSpinTimer;
    
    
    [SerializeField]
    public float InvencibleDuration;
    
    [SerializeField]
    public float InvencibleTimer;

    [SerializeField]
    public bool isDead;



    public void SetDashAttacking(bool value) => IsDashAttacking = value;
    public void SetJumping(bool value) => IsJumping = value;

    public void SetAttackSpin(bool value) => isAttackSpin = value;

    public void SetInvencible(bool value) => IsInvencible = value;

    public void SetFastSpin(bool value) => IsFastSpin = value;

    public void SetIsDead(bool value) => isDead = value;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        IsAttacking = isAttackSpin || IsDashAttacking;

        //Invencible
        if (InvencibleTimer > 0)
        {
            InvencibleTimer -= Time.deltaTime;
        }
        else
        { 
            SetInvencible(false);
        }

        //FastSpin
        if (FastSpinTimer > 0)
        {
            FastSpinTimer -= Time.deltaTime;
        }
        else
        {
            SetFastSpin(false);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PowerUp>() != null)
        {
            other.gameObject.GetComponent<PowerUp>().Interact(this.gameObject);
        }
    }
    public void ResetInvencibleTimer()
    {
        InvencibleTimer = InvencibleDuration;
    }

    public void ResetFastSpinTimer()
    {
        FastSpinTimer = FastSpinDuration;
    }


}

