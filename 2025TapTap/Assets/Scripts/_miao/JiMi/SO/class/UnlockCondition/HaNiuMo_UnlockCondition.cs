using UnityEngine;

namespace miao
{
    [CreateAssetMenu(menuName = "JiMi/UnlockConditions/HaNiuMo")]
    public class HaNiuMo_UnlockCondition : JiMiUnlockCondition
    {

        [SerializeField] private int requiredCount = 10;

        public override void OnEventTriggered(Player player)
        {
            //被调用的检测

        }

        public override bool IsSatisfied(Player player)
        {
            //满足的条件
            return Player.Instance.haQiCount >= requiredCount;

        }

        public override string GetProgressDescription()
        {
            return $"哈气次数 {Player.Instance.haQiCount}/{requiredCount}";
        }
    }
}
