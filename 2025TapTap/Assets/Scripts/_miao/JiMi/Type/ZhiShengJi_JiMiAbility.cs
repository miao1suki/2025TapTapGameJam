using UnityEngine;

public class ZhiShengJi_JiMiAbility : JiMiAbility
{
    public override void OnActivate(Player player)
    {
        Debug.Log("����ֱ������������ʼ���У�");
    }

    public override void OnDeactivate(Player player)
    {
        Debug.Log("�ر�ֱ����������ֹͣ���С�");
    }
}
