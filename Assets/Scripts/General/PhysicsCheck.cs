using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    public bool isGround;
    public LayerMask groundLayer;
    public Vector2 bottomOffset;
    public float checkRadius;
    private void Update() {
        Check();
    }
    public void Check(){
        //检测地面
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset,checkRadius,groundLayer);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset,checkRadius);
    }
}
