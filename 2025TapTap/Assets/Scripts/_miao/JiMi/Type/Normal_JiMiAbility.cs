using UnityEngine;

namespace miao
{
    public class Normal_JiMiAbility : JiMiAbility
    {
        public override void OnActivate(Player player)
        {
            Debug.Log("������ͨ��������������");
        }

        public override void OnDeactivate(Player player)
        {
            Debug.Log("�ر���ͨ�����������ָ���ͨ״̬��");
        }
    }
}
