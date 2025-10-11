using UnityEngine;

namespace miao
{
    public abstract class JiMiUnlockCondition : ScriptableObject
    {
        // 检查是否满足解锁条件
        public abstract bool IsSatisfied(Player player);

        // 当条件变化时调用，比如哈气次数更新、掉洞后触发
        public virtual void OnEventTriggered(Player player) { }

        // 返回当前进度字符串（比如“哈气次数 7/10”）
        public virtual string GetProgressDescription() => "";
    }
}
