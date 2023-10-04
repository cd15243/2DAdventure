using UnityEngine;
public class BoarChaseState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        Debug.Log("Enter Chase");
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        currentEnemy.anim.SetBool("run", true);
    }
    public override void LogicUpdate()
    {
        if(currentEnemy.lostTimeCounter <= 0){
            currentEnemy.SwitchState(NPCState.Patrol);
        }

        if(!currentEnemy.physicsCheck.isGround || (currentEnemy.physicsCheck.isTouchLeftWall && currentEnemy.faceDir.x < 0) || (currentEnemy.physicsCheck.isTouchRightWall && currentEnemy.faceDir.x > 0)){
            currentEnemy.transform.localScale = new Vector3(currentEnemy.faceDir.x,1,1);
        }
    }

    public override void PhysicsUpdate()
    {

    }

    public override void OnExit()
    {
        Debug.Log("Exit Chase");
        currentEnemy.lostTimeCounter = currentEnemy.lostTime;
        currentEnemy.anim.SetBool("run", false);
    }
}
