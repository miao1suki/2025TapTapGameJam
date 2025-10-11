using UnityEngine;

namespace miao
{
    public class Normal_JiMiAbility : JiMiAbility
    {
        public override void OnActivate(Player player)
        {
            Debug.Log("激活普通基米能力：哈！");
        }

        public override void OnDeactivate(Player player)
        {
            Debug.Log("关闭普通基米能力：恢复普通状态。");
        }
    }
}
