using UnityEngine;


[CreateAssetMenu(menuName = "JiMi/UnlockConditions/Default")]
public class Default_UnlockCondition : JiMiUnlockCondition
{

    public override void OnEventTriggered(Player player)
    {
        //�����õļ��
 
    }

    public override bool IsSatisfied(Player player)
    {
        //���������
        return true;
    }

    public override string GetProgressDescription()
    {
        return " Ĭ�ϻ����������";
    }
}
