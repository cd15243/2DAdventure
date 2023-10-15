using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    public PlayerInputControl inputControl;
    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider2D;
    private PhysicsCheck physicsCheck;
    private PlayerAnimation playerAnimation;
    public Vector2 inputDirection;
    [Header("基本参数")]
    public float speed;
    public float jumpForce;
    public float wallJumpForce;
    private float runSpeed;
    private float walkSpeed => speed/2.5f;
    public bool isCrouch;
    public bool isHurt;
    public bool isDead;
    public bool isAttack;
    public float hurtForce;
    public bool wallJump;
    public bool isSlide;
    private Vector2 originalOffset;
    private Vector2 originalSize;
    public float slideDistance;
    public float slideSpeed;
    public int slidePowerCost;

    private Character character;

    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        inputControl = new PlayerInputControl();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
        character = GetComponent<Character>();
        originalOffset = capsuleCollider2D.offset;
        originalSize = capsuleCollider2D.size;

#region 强制走路
        runSpeed = speed;
        inputControl.Gameplay.Jump.started += Jump;
        inputControl.Gameplay.WalkButton.performed += ctx => {
            if(physicsCheck.isGround){
                speed = walkSpeed;
            }
        };

        inputControl.Gameplay.WalkButton.canceled += ctx => {
            if(physicsCheck.isGround){
                speed = runSpeed;
            }
        };
#endregion
    
        inputControl.Gameplay.Attack.started += PlayerAttack;

        inputControl.Gameplay.Slide.started += PlayerSlide;
    }

    private void OnEnable() {
        inputControl.Enable();
    }

    private void Update() {
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();

        CheckState();
    }

    private void FixedUpdate() {
        if(!isHurt && !isAttack){
            Move();
        }
    }

    public void Move(){
        if(!isCrouch && !wallJump){
            rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime,rb.velocity.y);
        }
        int faceDir = (int)transform.localScale.x;
        if(inputDirection.x > 0){
            faceDir = 1;
        }
        if(inputDirection.x < 0){
            faceDir = -1;
        }

        //人物翻转
        transform.localScale = new Vector3(faceDir, 1, 1);

        //下蹲
        if(inputDirection.y < -0.5 && physicsCheck.isGround){
            isCrouch = true;
        }
        else{
            isCrouch = false;
        }

        if(isCrouch){
            capsuleCollider2D.offset = new Vector2(originalOffset.x,0.85f);
            capsuleCollider2D.size = new Vector2(originalSize.x,1.7f);
        }
        else{
            capsuleCollider2D.offset = originalOffset;
            capsuleCollider2D.size = originalSize;
        }
    }

    public void GetHurt(Transform attacker){
        isHurt = true;
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x),0).normalized;
        rb.AddForce(dir * hurtForce,ForceMode2D.Impulse);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if(physicsCheck.isGround){
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);

            isSlide = false;
            StopAllCoroutines();
        }
        else if(physicsCheck.isOnWall){
            rb.AddForce(new Vector2(-inputDirection.x,2f) * wallJumpForce, ForceMode2D.Impulse);
            wallJump = true;
        }
    }
    public void PlayerDead()
    {
        isDead = true;
        inputControl.Gameplay.Disable();
    }

    private void PlayerAttack(InputAction.CallbackContext context)
    {
        if(!physicsCheck.isGround){
            return;
        }
        playerAnimation.PlayAttack();
        isAttack = true;
    }

    private void PlayerSlide(InputAction.CallbackContext context)
    {
        if(!isSlide && physicsCheck.isGround && character.currentPower >= slidePowerCost){
            isSlide = true;

            var targetPos = new Vector3(transform.position.x + slideDistance * transform.localScale.x,transform.position.y);
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            StartCoroutine(TriggerSlide(targetPos));

            character.OnSlide(slidePowerCost);
        }
    }

    private IEnumerator TriggerSlide(Vector3 target){
        do{
            yield return null;
            if(!physicsCheck.isGround){
                break;
            }

            if(physicsCheck.isTouchLeftWall && transform.localScale.x < 0f || physicsCheck.isTouchRightWall && transform.localScale.x > 0f){
                isSlide = false;
                yield break;
            }

            rb.MovePosition(new Vector3(transform.position.x + slideSpeed * transform.localScale.x,transform.position.y));
        }
        while(Mathf.Abs(transform.position.x - target.x) > 0.1f);

        isSlide = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private void CheckState(){
        capsuleCollider2D.sharedMaterial = physicsCheck.isGround ? normal : wall;

        if(physicsCheck.isOnWall){
            rb.velocity = new Vector2(rb.velocity.x,rb.velocity.y/2f);
        }
        else{
            rb.velocity = new Vector2(rb.velocity.x,rb.velocity.y);
        }

        if(wallJump && rb.velocity.y < 0){
            wallJump = false;
        }
    }

    private void OnDisable() {
        inputControl.Disable();
    }
}
