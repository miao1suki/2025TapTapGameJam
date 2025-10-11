using UnityEngine;

namespace miao
{
    public abstract class JiMiAbility : MonoBehaviour
    {
        // 初始化时调用（如启用飞行模式、变色等）
        public abstract void OnActivate(Player player);

        // 禁用时调用（如关闭特效、恢复默认状态）
        public abstract void OnDeactivate(Player player);

        // 每帧更新（如能力需要持续作用）
        public virtual void OnUpdate(Player player) { }
    }
}