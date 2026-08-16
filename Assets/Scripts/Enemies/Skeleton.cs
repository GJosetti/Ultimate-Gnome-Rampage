using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class Skeleton : BaseEnemy
{
    [SerializeField] float damage;
    [SerializeField, Range(1, 20)] float attackRange;
    [SerializeField, Range(0, 2)] float attackStunTime = 0.5f;
    [SerializeField, Range(1, 20)] float attackForce;
    [SerializeField, Range(1, 20)] float attackCooldown;
    float attackTimer;

    Rigidbody rb;



    Animator animator;

    SkeletonState currentState;




    enum SkeletonState
    {
        Hide,
        Idle,
        Walk,
        Attack,
        Die
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.speed = 0;
        maxLife = 10;
        life = maxLife;
        currentState = SkeletonState.Hide;
        rb = GetComponent<Rigidbody>();

    }
    // Lógica
    void Update()
    {
        switch (currentState)
        {

            case SkeletonState.Hide:
                if (myRoom == GameManager.room)
                {
                    StartCoroutine(Appear());
                }
                break;


            case SkeletonState.Idle:
                if (attackTimer <= 0)
                {
                    currentState = SkeletonState.Walk;
                    break;
                }
                attackTimer -= Time.deltaTime;
                break;


            case SkeletonState.Walk:
               
                break;


            case SkeletonState.Attack:
                break;


            case SkeletonState.Die:
                break;
        }
    }

   //Física
    void FixedUpdate()
    {
        switch (currentState)
        {
            case SkeletonState.Walk:
                Vector3 dir = GetPlayer().transform.position - transform.position;
                dir.y = 0;

                if (dir != Vector3.zero)
                {
                    rb.MoveRotation(Quaternion.LookRotation(dir));
                }
                break;
        }
    }

    IEnumerator Appear()
    {
        yield return new WaitForSeconds(0.5f);
        animator.speed = 1;
        animator.GetCurrentAnimatorClipInfo(0);
        onAppearAnimationEnd();
    }
    void onAppearAnimationEnd()
    {
        currentState = SkeletonState.Idle;
    }
}