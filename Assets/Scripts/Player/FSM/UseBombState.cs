using UnityEngine;
//using System.Diagnostics;

public class UseBombState : PlayerBaseState
{
    public override void EnterState(PlayerControl player)//进入时调用
    {
        
    }

    public override void OnAttack(PlayerControl player)//攻击帧
    {
        player.Attack();
    }

}
