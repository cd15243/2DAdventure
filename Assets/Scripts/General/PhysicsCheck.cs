using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    private CapsuleCollider2D capsuleCollider2D;
    public bool isGround;
    public Vector2 leftOffset;
    public Vector2 rightOffset;
    public LayerMask groundLayer;
    public Vector2 bottomOffset;
    public bool manual;
    public bool isPlayer;
    private PlayerController playerController;
    private Rigidbody2D rb;
    public bool isOnWall;
    public float checkRaduis;
    public bool isTouchLeftWall;
    public bool isTouchRightWall;
    private void Awake() {
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        if(!manual){
            rightOffset = new Vector2((capsuleCollider2D.bounds.size.x + capsuleCollider2D.offset.x)/2,capsuleCollider2D.bounds.size.y/2);
            leftOffset = new Vector2(-rightOffset.x,rightOffset.y);
        }

        if(isPlayer){
            playerController = GetComponent<PlayerController>();
        }
    }
    private void Update() {
        Check();
    }
    public void Check(){
        //检测地面
        if(isOnWall){
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x,bottomOffset.y),checkRaduis,groundLayer);
        }
        else{
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x,0),checkRaduis,groundLayer);
        }

        //检测左右墙壁
        isTouchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(leftOffset.x,leftOffset.y),checkRaduis,groundLayer);
        isTouchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(rightOffset.x, rightOffset.y), checkRaduis, groundLayer);
    
        if(isPlayer){
            isOnWall = ((isTouchLeftWall && playerController.inputDirection.x < 0f) || (isTouchRightWall && playerController.inputDirection.x > 0f)) && rb.velocity.y < 0f;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y), checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(leftOffset.x, leftOffset.y), checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(rightOffset.x, rightOffset.y), checkRaduis);
    }
}
