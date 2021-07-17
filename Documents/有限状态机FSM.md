# FSM状态机
在游戏中，怪物在未发现玩家或者炸弹的适合会在两个Point之间巡逻，如果发现玩家则会追击玩家，发现炸弹会对炸弹产生对应的操作，如果使用传统if-else结构的话，会使代码臃肿，可扩展性较差。
为此引入了状态机，让怪物在不同的状态之间进行切换，Enemy代码负责进行各种状态的切换，每个状态的具体操作则由各自独立的代码实现。
抽象类EnemyBaseState提供统一的接口来让Enemy调用。
```C#
public abstract class EnemyBaseState
{
    //刚进入某一状态时调用
    public abstract void EnterState(Enemy enemy);
    //之后的每帧调用
    public abstract void OnUpdate(Enemy enemy);
}
```
在Enemy代码中，由TransitionToState函数负责切换各种状态，Update方法中调用OnUpdate函数
```C#
EnemyBaseState currentState; //当前状态

//模式转换
public void TransitionToState(EnemyBaseState state)
{
    currentState = state;
    currentState.EnterState(this);
}
//每帧调用
public virtual void Update()
{
    currentState.OnUpdate(this);
    anim.SetInteger("state", animState);
}
```
在游戏中，怪物有两种状态：巡逻状态和追击状态。
```C#
public PatrolState patrolState = new PatrolState(); //巡逻状态

public AttackState attackState = new AttackState(); //攻击状态
```
在巡逻状态下，怪物会在两个Point之间移动，一旦发现了可攻击的目标，则会进入攻击状态
```C#
public class PatrolState : EnemyBaseState
{
    public override void EnterState(Enemy enemy)//进入时调用
    {
        //每次进入巡逻状态时，会向一个最远的Point移动
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

        if (Mathf.Abs(enemy.transform.position.x - 
            enemy.targetPoint.position.x) < 0.01f)//到达运动终点
        {
            //重新进入巡逻状态，调整面向，向另一个Point移动
            enemy.TransitionToState(enemy.patrolState);
        }

        if (enemy.attackList.Count != 0)//怪物视线范围内有可攻击对象
        {
            enemy.TransitionToState(enemy.attackState);//转换成攻击模式
        }
    }
}
```
在Enemy代码中，维护着attackList的List，负责统计怪物视线中有多少可攻击对象，当attackList的数目不为0时，进入攻击模式；attackList的数目为0时，退出攻击模式，进入巡逻模式。
```C#
public class AttackState : EnemyBaseState
{
    public override void EnterState(Enemy enemy)//进入时调用
    {
        enemy.animState = 2;
        enemy.targetPoint = enemy.attackList[0];
    }

    public override void OnUpdate(Enemy enemy)//每帧调用
    {
        if (enemy.attackList.Count == 0)//可攻击对象消失
        {
            enemy.TransitionToState(enemy.patrolState);//转换巡逻模式
        }

        if (enemy.attackList.Count > 1)//范围内有多个攻击对象
        {
            //在attackList中寻找距离怪物最近的攻击点
            for (int i = 1; i < enemy.attackList.Count; ++i)
            {
                if (Mathf.Abs(enemy.targetPoint.position.x - 
                              enemy.transform.position.x) >
                    Mathf.Abs(enemy.attackList[i].position.x - 
                              enemy.transform.position.x))
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
```
