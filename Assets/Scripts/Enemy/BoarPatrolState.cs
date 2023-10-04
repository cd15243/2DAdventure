using UnityEngine;

public class BoarPatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        Debug.Log("Enter Patrol");
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
    }
    public override void LogicUpdate()
    {
        if (currentEnemy.FoundPlayer()){
            currentEnemy.SwitchState(NPCState.Chase);
        }

        if(!currentEnemy.physicsCheck.isGround || (currentEnemy.physicsCheck.isTouchLeftWall && currentEnemy.faceDir.x < 0) || (currentEnemy.physicsCheck.isTouchRightWall && currentEnemy.faceDir.x > 0)){
            currentEnemy.wait = true;
            currentEnemy.anim.SetBool("walk", false);
        }
        else{
            currentEnemy.anim.SetBool("walk", true);
        }
    }
    public override void PhysicsUpdate()
    {

    }
    public override void OnExit()
    {
        Debug.Log("Exit Patrol");
        currentEnemy.anim.SetBool("walk", false);
    }
}
