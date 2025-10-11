using UnityEngine;


[CreateAssetMenu(menuName = "JiMi/UnlockConditions/Default")]
public class Default_UnlockCondition : JiMiUnlockCondition
{

    public override void OnEventTriggered(Player player)
    {
        //被调用的检测
 
    }

    public override bool IsSatisfied(Player player)
    {
        //满足的条件
        return true;
    }

    public override string GetProgressDescription()
    {
        return " 默认基米无需解锁";
    }
}
