using UnityEngine;

public class DingDongJi_JiMiAbility : JiMiAbility
{
    public override void OnActivate(Player player)
    {
        Debug.Log("激活盯洞基能力：遁地！");
    }

    public override void OnDeactivate(Player player)
    {
        Debug.Log("关闭盯洞基能力：恢复普通状态。");
    }
}
