using UnityEngine;

namespace miao
{
    public class HaNiuMo_JiMiAbility : JiMiAbility
    {
        public override void OnActivate(Player player)
        {
            Debug.Log("激活哈牛魔能力：撞击得分翻倍！");
        }

        public override void OnDeactivate(Player player)
        {
            Debug.Log("关闭哈牛魔能力：恢复普通状态。");
        }
    }
}
