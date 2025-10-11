using UnityEngine;

public class ZhiShengJi_JiMiAbility : JiMiAbility
{
    public override void OnActivate(Player player)
    {
        Debug.Log("激活直升基能力：开始飞行！");
    }

    public override void OnDeactivate(Player player)
    {
        Debug.Log("关闭直升基能力：停止飞行。");
    }
}
