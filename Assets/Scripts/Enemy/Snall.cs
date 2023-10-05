using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snall : Enemy
{
    protected override void Awake() {
        base.Awake();
        patrolState = new SnallPatrolState();
        skillState = new SnallSkillState();
    }
}
