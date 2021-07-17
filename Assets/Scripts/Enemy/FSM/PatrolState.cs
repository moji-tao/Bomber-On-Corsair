using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : EnemyBaseState
{
    public override void EnterState(Enemy enemy)//进入时调用
    {
        enemy.animState = 0;//怪物Idle
        enemy.SwitchPoint();//调整面向
    }

    public override void OnUpdate(Enemy enemy)//每帧调用
    {
        if (!enemy.anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))//持续移动
        {
            //移动
            enemy.animState = 1;
            enemy.MoveToTarget();
        }

        if (Mathf.Abs(enemy.transform.position.x - enemy.targetPoint.position.x) < 0.01f)//到达运动终点
        {
            enemy.TransitionToState(enemy.patrolState);
        }

        if (enemy.attackList.Count != 0)//怪物视线范围内有可攻击对象
        {
            enemy.TransitionToState(enemy.attackState);//转换成攻击模式
        }
    }
}
