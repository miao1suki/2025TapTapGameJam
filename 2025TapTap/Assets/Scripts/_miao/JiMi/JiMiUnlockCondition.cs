using UnityEngine;

namespace miao
{
    public abstract class JiMiUnlockCondition : ScriptableObject
    {
        // ����Ƿ������������
        public abstract bool IsSatisfied(Player player);

        // �������仯ʱ���ã���������������¡������󴥷�
        public virtual void OnEventTriggered(Player player) { }

        // ���ص�ǰ�����ַ��������硰�������� 7/10����
        public virtual string GetProgressDescription() => "";
    }
}
