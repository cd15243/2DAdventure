using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rb;
    [HideInInspector]public Animator anim;
    [HideInInspector]public PhysicsCheck physicsCheck;
    [Header("基本变量")]
    public float normalSpeed;
    public float chaseSpeed;
    [HideInInspector]public float currentSpeed;
    public Vector3 faceDir;
    public Transform attacker;
    public float hurtForce;

    [Header("检测")]
    public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;

    [Header("计时器")]
    public float waitTime;
    public float waitCounter;
    public bool wait;

    public bool isHurt;
    public bool isDead;
    public float lostTime;
    public float lostTimeCounter;

    private BaseState currentState;
    protected BaseState patrolState;
    protected BaseState chaseState;

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicsCheck = GetComponentInChildren<PhysicsCheck>();
        currentSpeed = normalSpeed;
        waitCounter = waitTime;
    }

    private void OnEnable() {
        currentState = patrolState;
        currentState.OnEnter(this);
    }

    private void Update() {
        faceDir = new Vector3(-transform.localScale.x, 0, 0);
        currentState.LogicUpdate();
        TimeCounter();
    }

    private void FixedUpdate() {
        if(!isHurt && !isDead && !wait){
            Move();
        }
        currentState.PhysicsUpdate();
    }

    private void OnDisable() {
        currentState.OnExit();
    }

    public virtual void Move(){
        rb.velocity = new Vector2(currentSpeed * faceDir.x * Time.deltaTime, rb.velocity.y);
    }

    public void TimeCounter(){
        if(wait){
            waitCounter -= Time.deltaTime;
            if(waitCounter <= 0){
                wait = false;
                waitCounter = waitTime;
                transform.localScale = new Vector3(faceDir.x,1,1);
            }
        }

        if(!FoundPlayer() && lostTimeCounter >= 0){
            lostTimeCounter -= Time.deltaTime;
        }
        // else{
        //     lostTimeCounter = lostTime;
        // }
    }

    public bool FoundPlayer(){
        return Physics2D.BoxCast(transform.position + (Vector3)centerOffset,checkSize,0,faceDir,checkDistance,attackLayer);
    }

    public void SwitchState(NPCState state){
        var newState = state switch
        {
            NPCState.Patrol => patrolState,
            NPCState.Chase => chaseState,
            _ => patrolState
        };

        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);
    }

    public void OnTakeDamage(Transform attackTrans){
        attacker = attackTrans;

        if(attacker.position.x - transform.position.x > 0){
            transform.localScale = new Vector3(-1,1,1);
        }
        if(attacker.position.x - transform.position.x < 0){
            transform.localScale = new Vector3(1,1,1);
        }

        isHurt = true;
        anim.SetBool("hurt", true);
        Vector2 dir = new Vector2(transform.position.x - attacker.position.x,0).normalized;
        rb.velocity = new Vector2(0,rb.velocity.y);
        StartCoroutine(OnHurt(dir));
    }

    private IEnumerator OnHurt(Vector2 dir){
        rb.AddForce(dir * hurtForce,ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        isHurt = false;
    }

    public void OnDead(){
        gameObject.layer = 2;
        anim.SetBool("dead", true);
        isDead = true;
    }

    public void DestoryAfterAnimation(){
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position + (Vector3)centerOffset + new Vector3(checkDistance * (-transform.localScale.x),0),0.2f);
    }
}
