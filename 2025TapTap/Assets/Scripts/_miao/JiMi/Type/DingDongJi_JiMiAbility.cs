using UnityEngine;

public class DingDongJi_JiMiAbility : JiMiAbility
{
    public override void OnActivate(Player player)
    {
        Debug.Log("��������������ݵأ�");
    }

    public override void OnDeactivate(Player player)
    {
        Debug.Log("�رն������������ָ���ͨ״̬��");
    }
}
