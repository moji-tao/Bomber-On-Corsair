using UnityEngine;

public class AttackState : EnemyBaseState
{
    public override void EnterState(Enemy enemy)//进入时调用
    {
        enemy.animState = 2;
        enemy.targetPoint = enemy.attackList[0];
    }

    public override void OnUpdate(Enemy enemy)//每帧调用
    {
        if (enemy.hasBomb)
        { 
            return;
        }
        if (enemy.attackList.Count == 0)//可攻击对象消失
        {
            enemy.TransitionToState(enemy.patrolState);//转换巡逻模式
        }

        if (enemy.attackList.Count > 1)//范围内有多个攻击对象
        {
            for (int i = 1; i < enemy.attackList.Count; ++i)//找寻最近的目标点
            {
                if (Mathf.Abs(enemy.targetPoint.position.x - enemy.transform.position.x) >
                    Mathf.Abs(enemy.attackList[i].position.x - enemy.transform.position.x))
                {
                    enemy.targetPoint = enemy.attackList[i];
                }
            }
        }

        if (enemy.attackList.Count == 1)//范围内只有一个对象攻击此对象
        {
            enemy.targetPoint = enemy.attackList[0];
        }

        if (enemy.targetPoint.CompareTag("Player"))//追寻玩家
            enemy.AttackAction();
        if (enemy.targetPoint.CompareTag("Bomb"))//追寻炸弹
            enemy.SkillAction();

        enemy.MoveToTarget();

    }
}
