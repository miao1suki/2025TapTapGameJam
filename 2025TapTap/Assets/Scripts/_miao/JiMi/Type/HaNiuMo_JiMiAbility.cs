using UnityEngine;

namespace miao
{
    public class HaNiuMo_JiMiAbility : JiMiAbility
    {
        public override void OnActivate(Player player)
        {
            Debug.Log("�����ţħ������ײ���÷ַ�����");
        }

        public override void OnDeactivate(Player player)
        {
            Debug.Log("�رչ�ţħ�������ָ���ͨ״̬��");
        }
    }
}
