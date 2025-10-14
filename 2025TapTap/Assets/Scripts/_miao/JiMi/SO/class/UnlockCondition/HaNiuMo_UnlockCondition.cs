using UnityEngine;

namespace miao
{
    [CreateAssetMenu(menuName = "JiMi/UnlockConditions/HaNiuMo")]
    public class HaNiuMo_UnlockCondition : JiMiUnlockCondition
    {

        [SerializeField] private int requiredCount = 10;

        public override void OnEventTriggered(Player player)
        {
            //�����õļ��

        }

        public override bool IsSatisfied(Player player)
        {
            //���������
            return Player.Instance.haQiCount >= requiredCount;

        }

        public override string GetProgressDescription()
        {
            return $"�������� {Player.Instance.haQiCount}/{requiredCount}";
        }
    }
}
