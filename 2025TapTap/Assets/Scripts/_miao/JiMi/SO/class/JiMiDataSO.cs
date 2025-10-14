using UnityEngine;

namespace miao
{
    [CreateAssetMenu(fileName = "NewJiMiData", menuName = "JiMi/JiMi Data", order = 0)]
    public class JiMiDataSO : ScriptableObject
    {
        public JiMiType Type;  //哈基米类型
        public string DisplayName;  //显示的名称
        public Sprite Icon;  //图标
        public float SpeedMultiplier = 1.00f;  //速度倍率
        public float JumpPower = 1.00f;  //跳跃强度
        public float Gravity = 1.00f;  //重力系数



        public bool IsUnlocked = false;  // 是否解锁
        [TextArea]
        public string Description;  // 解锁条件说明

        [Header("解锁条件")]
        public JiMiUnlockCondition UnlockCondition;  // 对应的解锁条件
    }
}
