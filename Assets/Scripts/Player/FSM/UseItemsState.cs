using System.Security.Principal;
using UnityEngine;

public class UseItemsState : PlayerBaseState
{
    public override void EnterState(PlayerControl player)//进入时调用
    {

    }

    public override void OnAttack(PlayerControl player)//攻击帧
    {
        player.idem.GetComponent<Idem>().Use();
    }
}
