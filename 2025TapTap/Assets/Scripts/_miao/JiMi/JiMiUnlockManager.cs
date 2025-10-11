using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JiMiUnlockManager : MonoBehaviour
{
    public static JiMiUnlockManager Instance;  //单例

    public List<JiMiDataSO> allJiMis;
    private void Awake()
    {
        Instance = this;

        // 动态加载所有 JiMiDataSO
        JiMiDataSO[] loadedJiMis = Resources.LoadAll<JiMiDataSO>("JiMiData");
        allJiMis = new List<JiMiDataSO>(loadedJiMis);
    }
    public void OnPlayerEvent(Player player)
    {
        //事件变化时需调用
        foreach (var jimi in allJiMis)
        {
            if (jimi.UnlockCondition != null)
                jimi.UnlockCondition.OnEventTriggered(player);
        }
    }

    public void CheckUnlocks(Player player)
    {
        //检测是否满足解锁条件（用于检测是否应该切换至解锁）
        foreach (var jimi in allJiMis)
        {
            if (!jimi.IsUnlocked && jimi.UnlockCondition != null)
            {
                if (jimi.UnlockCondition.IsSatisfied(player))
                {
                    Unlocke(jimi);
                    Debug.Log($"{jimi.DisplayName} 已解锁！");
                    // 调用 UI、保存进度、更新状态等
                }
            }
        }
    }

    public bool IsUnlocked(string jimiName)
    {
        //获取IsUnlocked的状态（无法用于解锁，只是查看当前状态）
        var jimi = allJiMis.Find(j => j.name == jimiName);
        return jimi != null && jimi.IsUnlocked;
    }

    private void Unlocke(JiMiDataSO jimi)
    {
        jimi.IsUnlocked = true;
    }
}
