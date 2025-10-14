using UnityEngine;

namespace miao
{
    [CreateAssetMenu(fileName = "NewJiMiData", menuName = "JiMi/JiMi Data", order = 0)]
    public class JiMiDataSO : ScriptableObject
    {
        public JiMiType Type;  //����������
        public string DisplayName;  //��ʾ������
        public Sprite Icon;  //ͼ��
        public float SpeedMultiplier = 1.00f;  //�ٶȱ���
        public float JumpPower = 1.00f;  //��Ծǿ��
        public float Gravity = 1.00f;  //����ϵ��



        public bool IsUnlocked = false;  // �Ƿ����
        [TextArea]
        public string Description;  // ��������˵��

        [Header("��������")]
        public JiMiUnlockCondition UnlockCondition;  // ��Ӧ�Ľ�������
    }
}
