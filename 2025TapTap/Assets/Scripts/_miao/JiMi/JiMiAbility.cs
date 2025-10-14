using UnityEngine;

namespace miao
{
    public abstract class JiMiAbility : MonoBehaviour
    {
        // ��ʼ��ʱ���ã������÷���ģʽ����ɫ�ȣ�
        public abstract void OnActivate(Player player);

        // ����ʱ���ã���ر���Ч���ָ�Ĭ��״̬��
        public abstract void OnDeactivate(Player player);

        // ÿ֡���£���������Ҫ�������ã�
        public virtual void OnUpdate(Player player) { }
    }
}