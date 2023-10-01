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
    public Vector2 inputDirection;
    [Header("基本参数")]
    public float speed;
    public float jumpForce;
    private float runSpeed;
    private float walkSpeed => speed/2.5f;
    public bool isCrouch;
    public bool isHurt;
    public bool isDead;
    public float hurtForce;
    private Vector2 originalOffset;
    private Vector2 originalSize;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        inputControl = new PlayerInputControl();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
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
    }



    private void OnEnable() {
        inputControl.Enable();
    }

    private void Update() {
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate() {
        if(!isHurt){
            Move();
        }
    }

    public void Move(){
        if(!isCrouch){
            rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime,rb.velocity.y);
        }
        
        //人物翻转
        transform.localScale = new Vector3(inputDirection.x > 0 ? 1 : -1, 1, 1);

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
        }
    }
    public void PlayerDead()
    {
        isDead = true;
        inputControl.Gameplay.Disable();
    }

    private void OnDisable() {
        inputControl.Disable();
    }
}
